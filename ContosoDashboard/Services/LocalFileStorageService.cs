using System.IO;
using Microsoft.Extensions.Configuration;

namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private readonly HashSet<string> _allowedExtensions;

    public LocalFileStorageService(IConfiguration configuration)
    {
        var configuredRoot = configuration["DocumentStorage:RootPath"];
        _rootPath = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ContosoDashboard", "uploads")
            : Path.GetFullPath(configuredRoot);

        _allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt",
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".csv"
        };

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, int userId, int? projectId = null)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var normalizedFileName = string.IsNullOrWhiteSpace(fileName) ? "upload.bin" : Path.GetFileName(fileName);
        var extension = Path.GetExtension(normalizedFileName);
        if (string.IsNullOrWhiteSpace(extension) || !await IsAllowedExtensionAsync(normalizedFileName))
        {
            throw new InvalidOperationException("Unsupported file type.");
        }

        var safeName = $"{Guid.NewGuid():N}{extension}";
        var projectSegment = projectId.HasValue ? projectId.Value.ToString() : "personal";
        var relativeFolder = Path.Combine("users", userId.ToString(), projectSegment);
        var targetDirectory = Path.Combine(_rootPath, relativeFolder);

        Directory.CreateDirectory(targetDirectory);

        var targetPath = Path.Combine(targetDirectory, safeName);
        await using var output = File.Create(targetPath);
        await content.CopyToAsync(output);

        return Path.Combine(relativeFolder, safeName).Replace('\\', '/');
    }

    public Task<Stream> DownloadAsync(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException("Storage path is required.", nameof(storagePath));
        }

        var fullPath = Path.Combine(_rootPath, storagePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The requested file was not found.", fullPath);
        }

        return Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    public Task DeleteAsync(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return Task.CompletedTask;
        }

        var fullPath = Path.Combine(_rootPath, storagePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<string> GetUrlAsync(string storagePath, TimeSpan? expiration = null)
    {
        return Task.FromResult(storagePath);
    }

    public Task<bool> IsAllowedExtensionAsync(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return Task.FromResult(!string.IsNullOrWhiteSpace(extension) && _allowedExtensions.Contains(extension));
    }
}
