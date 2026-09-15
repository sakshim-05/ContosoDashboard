namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, int userId, int? projectId = null);
    Task<Stream> DownloadAsync(string storagePath);
    Task DeleteAsync(string storagePath);
    Task<string> GetUrlAsync(string storagePath, TimeSpan? expiration = null);
    Task<bool> IsAllowedExtensionAsync(string fileName);
}
