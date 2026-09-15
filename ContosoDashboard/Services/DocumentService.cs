using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<DocumentUploadResult> UploadDocumentAsync(DocumentUploadRequest request, Stream content, string fileName, string contentType, int currentUserId);
    Task<List<Document>> GetUserDocumentsAsync(int userId);
    Task<List<Document>> GetProjectDocumentsAsync(int projectId, int requestingUserId);
    Task<Document?> GetDocumentByIdAsync(int documentId, int requestingUserId);
    Task<bool> CanUserAccessDocumentAsync(int documentId, int requestingUserId);
}

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly IProjectService _projectService;
    private readonly INotificationService _notificationService;

    public DocumentService(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        IProjectService projectService,
        INotificationService notificationService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _projectService = projectService;
        _notificationService = notificationService;
    }

    public async Task<DocumentUploadResult> UploadDocumentAsync(DocumentUploadRequest request, Stream content, string fileName, string contentType, int currentUserId)
    {
        if (request == null)
        {
            return DocumentUploadResult.CreateFailure("Upload request is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return DocumentUploadResult.CreateFailure("Document title is required.");
        }

        if (content == null || content.Length == 0)
        {
            return DocumentUploadResult.CreateFailure("A file is required to upload.");
        }

        if (content.Length > 25 * 1024 * 1024)
        {
            return DocumentUploadResult.CreateFailure("The selected file exceeds the 25 MB limit.");
        }

        if (!await _fileStorageService.IsAllowedExtensionAsync(fileName))
        {
            return DocumentUploadResult.CreateFailure("Unsupported file type. Use a PDF, image, text, or office document.");
        }

        if (request.ProjectId.HasValue)
        {
            var canAccessProject = await _projectService.UserHasProjectAccessAsync(request.ProjectId.Value, currentUserId);
            if (!canAccessProject)
            {
                return DocumentUploadResult.CreateFailure("You are not a member of the selected project.");
            }
        }

        try
        {
            var storagePath = await _fileStorageService.UploadAsync(content, fileName, contentType, currentUserId, request.ProjectId);

            var document = new Document
            {
                Title = request.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                Category = request.Category,
                ProjectId = request.ProjectId,
                TaskId = request.TaskId,
                UploadedByUserId = currentUserId,
                StoragePath = storagePath,
                OriginalFileName = Path.GetFileName(fileName),
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
                FileSizeBytes = content.Length,
                UploadedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Tags = request.Tags
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            if (request.ProjectId.HasValue)
            {
                var projectMembers = await _context.ProjectMembers
                    .Where(pm => pm.ProjectId == request.ProjectId.Value)
                    .Select(pm => pm.UserId)
                    .Distinct()
                    .ToListAsync();

                foreach (var memberId in projectMembers.Where(id => id != currentUserId))
                {
                    await _notificationService.CreateNotificationAsync(new Notification
                    {
                        UserId = memberId,
                        Title = "New project document",
                        Message = $"{request.Title} was uploaded to the project.",
                        Type = NotificationType.ProjectUpdate,
                        Priority = NotificationPriority.Informational
                    });
                }
            }

            return DocumentUploadResult.CreateSuccess(document);
        }
        catch (Exception ex)
        {
            return DocumentUploadResult.CreateFailure($"Upload failed: {ex.Message}");
        }
    }

    public async Task<List<Document>> GetUserDocumentsAsync(int userId)
    {
        return await _context.Documents
            .Include(d => d.Project)
            .Include(d => d.UploadedByUser)
            .Where(d => d.UploadedByUserId == userId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<List<Document>> GetProjectDocumentsAsync(int projectId, int requestingUserId)
    {
        var hasAccess = await _projectService.UserHasProjectAccessAsync(projectId, requestingUserId);
        if (!hasAccess)
        {
            return new List<Document>();
        }

        return await _context.Documents
            .Include(d => d.Project)
            .Include(d => d.UploadedByUser)
            .Where(d => d.ProjectId == projectId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId, int requestingUserId)
    {
        var document = await _context.Documents
            .Include(d => d.Project)
            .Include(d => d.UploadedByUser)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null)
        {
            return null;
        }

        if (document.UploadedByUserId == requestingUserId)
        {
            return document;
        }

        if (document.ProjectId.HasValue && await _projectService.UserHasProjectAccessAsync(document.ProjectId.Value, requestingUserId))
        {
            return document;
        }

        return null;
    }

    public async Task<bool> CanUserAccessDocumentAsync(int documentId, int requestingUserId)
    {
        var document = await _context.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DocumentId == documentId && !d.IsDeleted);

        if (document == null)
        {
            return false;
        }

        if (document.UploadedByUserId == requestingUserId)
        {
            return true;
        }

        if (document.ProjectId.HasValue)
        {
            return await _projectService.UserHasProjectAccessAsync(document.ProjectId.Value, requestingUserId);
        }

        return false;
    }
}

public class DocumentUploadResult
{
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Document? Document { get; private set; }

    public static DocumentUploadResult CreateSuccess(Document document)
    {
        return new DocumentUploadResult { Success = true, Document = document };
    }

    public static DocumentUploadResult CreateFailure(string errorMessage)
    {
        return new DocumentUploadResult { Success = false, ErrorMessage = errorMessage };
    }
}
