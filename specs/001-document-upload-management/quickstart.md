# Quickstart: Document Upload and Management Validation

## Prerequisites

- .NET 10 SDK installed
- Local SQL Server or a working connection string configured in `ContosoDashboard/appsettings.Development.json`
- Access to the existing dashboard application with a seeded user account
- A small set of sample documents, including one valid PDF and one unsupported file type

## Setup

1. Open the solution and confirm the app configuration points to a valid local database.
2. Restore dependencies with:
   `dotnet restore`
3. Start the application with:
   `dotnet run --project ContosoDashboard/ContosoDashboard.csproj`
4. Sign in as an existing seeded user or a known project member.

## Validation Scenarios

### 1. Valid upload flow

- Navigate to the document upload experience.
- Select a supported file (for example, a PDF under 25 MB).
- Fill in a title, category, and optional tags or project association.
- Submit the upload.
- Expected result: success message appears, the document is stored in the secure upload folder, and it appears in the user’s document list.

### 2. Invalid upload rejection

- Attempt to upload a file over 25 MB or with an unsupported extension.
- Expected result: submission is blocked, and a clear user-facing validation message explains the issue.

### 3. Project visibility and authorization

- Upload a project document for a project where the current user is a member.
- Sign in as another authorized project member.
- Expected result: the document appears in the project view for authorized users and is hidden from unauthorized users.

### 4. Search and filtering

- Search by title, category, project name, or uploader name.
- Filter by date range or category.
- Expected result: only authorized records appear, and the results reflect the selected criteria.

### 5. Sharing flow

- Share a document with another user who is not part of the project.
- Expected result: the share is recorded, the recipient receives an in-app notification, and the document appears in their shared-with-me area.

### 6. Delete and replace flow

- Edit metadata or replace an uploaded file with an updated version.
- Confirm deletion for an owned document.
- Expected result: updated metadata is visible, the file version changes appropriately, and the final deletion removes the document from authorized lists.

## Verification Commands

- `dotnet build ContosoDashboard/ContosoDashboard.csproj`
- `dotnet test` (when targeted tests are added for document services)
- Manual browser validation using the upload, search, project, and shared-documents views

## Expected Outcomes

- No direct file access from the static web root
- Valid uploads complete and appear in the correct document views
- Unauthorized access is denied before file content is served
- Project and sharing-based filters behave as required by the feature specification
