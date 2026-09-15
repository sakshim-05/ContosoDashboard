using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DocumentCategory Category { get; set; } = DocumentCategory.Other;

    public int? ProjectId { get; set; }

    public int? TaskId { get; set; }

    [Required]
    public int UploadedByUserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string StoragePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string ContentType { get; set; } = "application/octet-stream";

    [Required]
    public long FileSizeBytes { get; set; }

    [MaxLength(255)]
    public string Tags { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public int VersionNumber { get; set; } = 1;

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    [ForeignKey("UploadedByUserId")]
    public virtual User? UploadedByUser { get; set; }

    [ForeignKey("TaskId")]
    public virtual TaskItem? Task { get; set; }

    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
}

public class DocumentShare
{
    [Key]
    public int DocumentShareId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    public int? UserId { get; set; }

    [MaxLength(100)]
    public string? TeamId { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    public DateTime SharedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    public string PermissionLevel { get; set; } = "View";

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User? SharedWithUser { get; set; }

    [ForeignKey("SharedByUserId")]
    public virtual User? SharedByUser { get; set; }
}

public class DocumentUploadRequest
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DocumentCategory Category { get; set; } = DocumentCategory.Other;

    public int? ProjectId { get; set; }

    public int? TaskId { get; set; }

    [MaxLength(255)]
    public string Tags { get; set; } = string.Empty;
}

public enum DocumentCategory
{
    ProjectDocuments,
    TeamResources,
    PersonalFiles,
    Reports,
    Presentations,
    Other
}
