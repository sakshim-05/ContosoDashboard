# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish secure local storage and the foundation required for document metadata work.

- [ ] T001 Configure the application storage root and document upload settings in ContosoDashboard/appsettings.json
- [ ] T002 Create the storage contract for local file handling in ContosoDashboard/Services/IFileStorageService.cs
- [ ] T003 [P] Implement the local file storage service in ContosoDashboard/Services/LocalFileStorageService.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Add the shared document data model, service layer, and authorization primitives before story work begins.

- [ ] T004 Add the `Document` and `DocumentShare` entities and EF Core configuration in ContosoDashboard/Models/Document.cs and ContosoDashboard/Data/ApplicationDbContext.cs
- [ ] T005 [P] Add database indexes and validation rules for document metadata in ContosoDashboard/Data/ApplicationDbContext.cs
- [ ] T006 Implement document CRUD and authorization gate logic in ContosoDashboard/Services/DocumentService.cs
- [ ] T007 [P] Extend project visibility checks for project-scoped document access in ContosoDashboard/Services/ProjectService.cs
- [ ] T008 Add document-related notifications and audit hooks in ContosoDashboard/Services/NotificationService.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Upload and organize work documents (Priority: P1) 🎯 MVP

**Goal**: Allow authenticated users to upload supported files, store metadata securely, and see their documents in the dashboard/project context.

**Independent Test**: A user can choose a valid file, provide required metadata, complete upload, and then see the document in their own document list.

### Implementation for User Story 1

- [ ] T009 [P] [US1] Create the document upload model and validation rules in ContosoDashboard/Models/Document.cs
- [ ] T010 [US1] Implement upload validation, file-size enforcement, extension checks, and secure path generation in ContosoDashboard/Services/DocumentService.cs
- [ ] T011 [P] [US1] Add the document upload and list page in ContosoDashboard/Pages/Documents.razor
- [ ] T012 [US1] Register the document feature in the app startup flow and service container in ContosoDashboard/Program.cs
- [ ] T013 [P] [US1] Add the recent documents summary widget to the dashboard in ContosoDashboard/Pages/Index.razor and ContosoDashboard/Services/DashboardService.cs

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Find and access documents safely (Priority: P2)

**Goal**: Support search, filter, and authorization-aware access so users only see documents they can legitimately view or download.

**Independent Test**: A user can search by title, tags, or project and only sees documents they are allowed to access.

### Implementation for User Story 2

- [ ] T014 [US2] Add document query filters, project-aware search, and access checks in ContosoDashboard/Services/DocumentService.cs
- [ ] T015 [P] [US2] Add search and filter UX for documents in ContosoDashboard/Pages/Documents.razor
- [ ] T016 [US2] Enforce download and preview authorization before exposing files and metadata in ContosoDashboard/Services/DocumentService.cs
- [ ] T017 [P] [US2] Surface project-bound documents in project views and restrict them to authorized team members in ContosoDashboard/Pages/ProjectDetails.razor

**Checkpoint**: At this point, User Stories 1 and 2 should both work independently

---

## Phase 5: User Story 3 - Manage sharing and document lifecycle (Priority: P3)

**Goal**: Allow owners and managers to update document metadata, share documents, replace files, and remove stale records with proper notifications.

**Independent Test**: A user can edit sharing or metadata for a document and verify the updated record and notifications appear correctly.

### Implementation for User Story 3

- [ ] T018 [US3] Implement metadata updates, versioning, and replace-file handling in ContosoDashboard/Services/DocumentService.cs
- [ ] T019 [P] [US3] Add explicit share records and recipient notification creation in ContosoDashboard/Models/DocumentShare.cs and ContosoDashboard/Services/DocumentService.cs
- [ ] T020 [US3] Build shared-with-me and task attachment flows in ContosoDashboard/Pages/Documents.razor and ContosoDashboard/Pages/Tasks.razor
- [ ] T021 [US3] Add delete confirmation and audit logging for document removal in ContosoDashboard/Services/DocumentService.cs and ContosoDashboard/Services/NotificationService.cs

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate the end-to-end flow, tighten security, and ensure the feature is production-ready for the training environment.

- [ ] T022 [P] Run the quickstart validation steps from specs/001-document-upload-management/quickstart.md and fix any issues
- [ ] T023 Review and tighten security checks for file names, storage paths, and unauthorized access in ContosoDashboard/Services/DocumentService.cs and ContosoDashboard/Services/LocalFileStorageService.cs
- [ ] T024 [P] Improve document list performance and confirm indexes cover category, project, uploader, and date filters in ContosoDashboard/Data/ApplicationDbContext.cs
- [ ] T025 Update the README and feature documentation to reflect the document management workflow in README.md and StakeholderDocs/document-upload-and-management-feature.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion; may integrate with US1 but should remain independently testable
- **User Story 3 (Phase 5)**: Depends on Foundational completion; may integrate with US1/US2 but should remain independently testable
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2; no dependencies on other stories
- **US2 (P2)**: Can start after Phase 2; depends on the secure upload and project visibility foundation from US1
- **US3 (P3)**: Can start after Phase 2; depends on the metadata and sharing primitives established in US1 and US2

### Parallel Opportunities

- All Phase 1 tasks can run in parallel
- Most Phase 2 tasks can run in parallel once the storage contract exists
- US1 implementation tasks may proceed in parallel across UI, service, and dashboard work
- US2 search and project visibility work can proceed concurrently with the download access checks
- US3 share and delete flows can proceed independently once the document service is stable

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate the upload flow independently
5. Stop and confirm the feature is working before proceeding to later stories

### Incremental Delivery

1. Setup + Foundational → secure storage and document metadata foundation
2. US1 → upload, list, and dashboard visibility
3. US2 → secure search and download restrictions
4. US3 → sharing, replacement, and deletion lifecycle
5. Polish → validation, performance, and documentation

---

## Notes

- Tasks are organized by user story to preserve independent implementation and testing boundaries.
- `DocumentService` is the primary enforcement point for size, extension, project membership, and share authorization checks.
- `IFileStorageService` remains the migration seam for future cloud storage, while local disk storage remains the default in this training app.
