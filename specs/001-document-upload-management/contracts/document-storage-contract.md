# Document Storage Contract

## Purpose

This contract defines the storage abstraction used by the feature so the business logic can remain independent from the physical storage implementation.

## Interface Contract

```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, int userId, int? projectId = null);
    Task<Stream> DownloadAsync(string storagePath);
    Task DeleteAsync(string storagePath);
    Task<string> GetUrlAsync(string storagePath, TimeSpan? expiration = null);
    Task<bool> IsAllowedExtensionAsync(string fileName);
}
```

## Responsibilities

- `UploadAsync` should generate or accept a secure storage path, persist the file bytes, and return a safe relative path for metadata storage.
- `DownloadAsync` should return the bytes for an authorized file only.
- `DeleteAsync` should remove the underlying file and leave metadata cleanup to the calling service.
- `GetUrlAsync` provides a future migration point for Azure or other object storage.
- `IsAllowedExtensionAsync` validates the extension whitelist before the file is saved.

## Local Implementation Expectations

- Files are stored under a local application directory outside the web root.
- Storage paths are relative, portable, and safe for future cloud migration.
- The service must never trust a user-supplied file name for the final path; GUID-backed names are required.

## Business Rules Enforced Outside the Contract

- Authorization is required before file downloads or project document visibility.
- `DocumentService` must validate file size, extension, project membership, and sharing rules before persisting metadata.
- The file path must be generated prior to database insertion to avoid duplicate-key and orphaned-record problems.
