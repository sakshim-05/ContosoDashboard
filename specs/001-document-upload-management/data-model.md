# Data Model: Document Upload and Management

## Overview

This feature introduces document metadata and sharing entities while reusing the existing `User`, `Project`, `TaskItem`, and `Notification` models already present in the dashboard. The document system will store file metadata in SQL Server and file bytes in a local, secure storage service outside the web root.

## Entities

### Document

Represents a stored work document and its metadata.

- `DocumentId` (int, primary key)
- `Title` (string, required, max 255)
- `Description` (string?, optional, max 2000)
- `Category` (string, required, one of: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other)
- `ProjectId` (int?, optional foreign key to `Project`)
- `TaskId` (int?, optional foreign key to `TaskItem`)
- `UploadedByUserId` (int, required foreign key to `User`)
- `StoragePath` (string, required, max 500, relative path or blob name)
- `OriginalFileName` (string, required, max 255)
- `ContentType` (string, required, max 255)
- `FileSizeBytes` (long, required, > 0)
- `UploadedAt` (DateTime, required, UTC)
- `UpdatedAt` (DateTime, required, UTC)
- `IsDeleted` (bool, default false)
- `VersionNumber` (int, default 1)

Relationships:

- Many documents belong to one uploader (`User`)
- Many documents may belong to one project (`Project`)
- Many documents may be attached to one task (`TaskItem`)
- One document may have many share records (`DocumentShare`)

Validation rules:

- `Title` cannot be empty.
- `Category` must match a known list value.
- `ContentType` cannot exceed 255 characters.
- `StoragePath` must be generated before persistence and must not use user-controlled names.
- `FileSizeBytes` must be <= 25 MB.
- `ProjectId` must be valid when supplied and the user must be a project member or manager before attaching the document.

### DocumentShare

Represents an explicit access grant beyond the base project membership.

- `DocumentShareId` (int, primary key)
- `DocumentId` (int, required foreign key to `Document`)
- `UserId` (int?, optional foreign key to `User`)
- `TeamId` (string?, optional future extension for team-level grants)
- `SharedByUserId` (int, required foreign key to `User`)
- `SharedAt` (DateTime, UTC)
- `PermissionLevel` (string, default `View`)

Relationships:

- Many share records belong to one document.
- Each share record can target a single user or a future team-level scope.

Validation rules:

- At least one of `UserId` or `TeamId` must be provided.
- The owner or a permitted manager must be the `SharedByUserId`.
- `PermissionLevel` is validated against the allowed set (`View`, `Edit`, `Manage`).

### User

Existing entity; document-specific additions should include:

- one-to-many uploaded documents
- one-to-many explicit share grants created by the user

### Project

Existing entity; document-specific additions should include:

- many project documents
- project membership as the default access channel for project-bound documents

### TaskItem

Existing entity; document-specific additions should include:

- many task attachments
- optional association between a document and a project task for task context visibility

### Notification

Existing entity; document activity should create notifications for:

- new shared document for recipient
- project document uploaded for project members
- document deleted or updated when relevant to watchers

## State Transitions

### Document lifecycle

- Draft/Validation: file selected and metadata captured
- Ready for upload: validation passes; file is saved to storage service
- Stored: database record created and file bytes persisted
- Shared: explicit `DocumentShare` records created for recipients
- Updated: metadata changes or replacement file upload increments version and updates timestamps
- Deleted: soft-delete or hard-delete after confirmation, depending on audit policy; removed from authorized lists after persistence

## Data Integrity Notes

- File path generation must happen before database persistence to avoid duplicate key problems and orphaned records.
- Authorization checks are enforced in `DocumentService` before returning or modifying the record.
- The feature should use a local storage directory and keep the database schema portable for future migration.
