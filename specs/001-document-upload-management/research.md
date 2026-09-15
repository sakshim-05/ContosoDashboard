# Research: Document Upload and Management

## Research Objectives

Resolve the open technical decisions required to implement the feature while staying consistent with the repository’s existing Blazor Server, EF Core, and mock-auth architecture.

## Decision: Local filesystem storage with a storage abstraction

The repository will store uploaded content outside the web root using a dedicated local folder such as `AppData/uploads`. The actual database will keep metadata only, while the storage implementation uses a repository-level abstraction named `IFileStorageService` with `UploadAsync`, `DeleteAsync`, `DownloadAsync`, and `GetUrlAsync` methods.

### Rationale

- Matches the existing requirement to keep the training app offline and local-only.
- Prevents direct exposure from the static file pipeline and reduces path traversal risk.
- Preserves an Azure-ready migration pattern without forcing a cloud dependency in the training build.
- Fits the service-layer architecture already used by `ProjectService`, `TaskService`, and `NotificationService`.

### Alternatives considered

- Store files under `wwwroot`: rejected because it creates direct public access and violates the security requirement.
- Use database blob columns: rejected because the feature explicitly requires local file storage and a future migration path with interface-based storage abstraction.
- Add direct file I/O into the Razor page: rejected because it bypasses the existing service-layer patterns and weakens authorization enforcement.

## Decision: Document visibility enforced by project membership + explicit share records

Document access will be controlled by a combination of project membership rules and a `DocumentShare` relationship for users or groups beyond the project team.

### Rationale

- This supports the requirement that project documents are visible to all authorized project members by default.
- It also supports deeper sharing while preserving least exposure and explicit authorization.
- It maps cleanly to the current project membership model in `ProjectMember` and the existing notification flow.

### Alternatives considered

- Grant access to every authenticated user: rejected due to the security-by-design and user isolation principles.
- Store access rules only in project membership: rejected because document sharing beyond the team requires explicit user-level grants.

## Decision: Server-side filtering and indexing for search

Search and list operations should be implemented as EF Core queries against the metadata tables, with indexing on `Category`, `ProjectId`, `UploadedByUserId`, and `UploadedAt`.

### Rationale

- Keeps search fast for the expected dataset size.
- Matches the repository’s current repository/service patterns.
- Aligns with the requirement that users see only authorized results and that the app remains understandable for training.

### Alternatives considered

- Search in-memory across all documents: rejected because it scales poorly and does not respect authorization boundaries.
- Add a full-text search engine: rejected because the project is intentionally simple and local-only.

## Decision: Extend the existing notification model for document sharing

The document feature will reuse the existing notification service and in-app notification patterns to inform recipients when a document is shared or added to a project context.

### Rationale

- Reduces new infrastructure and keeps the dashboard behavior consistent.
- Supports both document-sharing and project-related document alerts.
- Aligns with the current `Notification` model and service-based architecture.

### Alternatives considered

- Build a separate notification subsystem: rejected because it adds unnecessary duplication.
- Send email-only notifications: rejected because the requirement is explicitly in-app notifications in the current training app.

## Decision: Keep Entity Framework metadata and concrete file path storage separate

The database (EF Core) will store document metadata and a relative storage path. File bytes remain in the storage service, not inside the database record.

### Rationale

- Supports the requirement for unique GUID-based filenames and secure local storage.
- Keeps the local/cloud abstraction simple.
- Prevents orphaned metadata when a file upload fails after the metadata record is created.

### Alternatives considered

- Save binary content directly in the database: rejected because it conflicts with the stated storage design and the migration requirement.
- Keep path and file content in the same object: rejected because it couples metadata and file handling and makes testing harder.

## Research Summary

The document feature is best implemented as a thin extension of the current service-based architecture: models for document metadata, a secure storage abstraction for local files, project-aware access checks, and notification-driven user experience. This approach satisfies the repository constitution, avoids unnecessary rewrites, and preserves the training-safe design.
