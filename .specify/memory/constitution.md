<!-- Sync Impact Report
- Version change: 0.0.0 -> 1.0.0
- Modified principles: none -> I. Security-by-Design, II. User Isolation and Least Exposure, III. Evidence-Driven Change, IV. Simplicity over Cleverness, V. Training-Ready Transparency
- Added sections: Security and Data Handling; Development Workflow
- Removed sections: none
- Deferred items: TODO(RATIFICATION_DATE): establish the original adoption date for the ContosoDashboard constitution.
-->

# ContosoDashboard Constitution

## Core Principles

### I. Security-by-Design
All repository changes must preserve the principle that this application is a training-oriented dashboard with explicit protection boundaries. Authentication, authorization, and data access must be enforced at the service layer and the page layer, and no change may introduce privilege escalation, insecure direct object references, or bypasses around role checks.

### II. User Isolation and Least Exposure
Each user must only access data and actions explicitly authorized for their role and team. The system must treat identity claims as the source of truth; any feature that exposes project, task, or notification data must validate membership, ownership, and role permissions before displaying or mutating data.

### III. Evidence-Driven Change
The project must evolve through reviewable, testable changes. New behavior requires a clear requirement, implementation that matches the requirement, and verification that the relevant behavior works before merge. The repository may not rely on undocumented "works on my machine" assumptions.

### IV. Simplicity over Cleverness
Architectural and code changes must favor clear, maintainable patterns over hidden abstraction. Components, services, and data models must have obvious responsibilities, readable naming, and limited coupling so future learners can understand the app and extend it safely.

### V. Training-Ready Transparency
This repository is a learning artifact. Documentation, naming, and implementation decisions must remain understandable to students, and examples must clearly distinguish training-safe patterns from production-grade security controls. Features must not claim production readiness when they are intentionally simplified for offline or educational use.

## Security and Data Handling
The application MUST remain suitable for offline training and local-only execution. No change may add hard external service dependencies without a clear migration story and explicit documentation. Storage, auth state, and file access must remain isolated to the local project environment unless the repository owner deliberately introduces a new architecture contract.

Security requirements:
- Authentication and authorization MUST be enforced in both UI and service layers.
- Role and member checks MUST be validated before any project, task, or notification data is read or changed.
- Secrets, credentials, and local user data MUST be kept out of committed source files and sample data.
- Mock authentication is acceptable only for training scenarios; production patterns require a real identity provider, hashed credentials, and compliant security controls.
- Data access patterns MUST avoid IDOR and cross-user leakage.

## Development Workflow
All work in this repository MUST be traceable to a clear task or requirement and must preserve the educational intent of the application. Before merging, the team must confirm that the change is consistent with the project goals, security boundaries, and maintainability standards defined here.

Required process:
- Feature work starts with a clear requirement and explicit acceptance criteria.
- Code must remain readable, consistent with existing conventions, and suitable for training use.
- Changes that affect auth, authorization, or data access require a deliberate review of permission checks and sample-data assumptions.
- Tests or verification steps MUST be run for the impacted behavior when practical, and unverified changes are not considered complete.
- Documentation must be updated when user-facing behavior, security assumptions, or architecture constraints change.

## Governance
This Constitution governs all work in the ContosoDashboard repository. It supersedes informal practices when a conflict exists, and any exception must be documented with a rationale and a reviewable migration or remediation plan.

Amendments require a written proposal, a documented rationale tied to repository goals or security needs, and review by the maintainers before adoption. Each amendment must be recorded in the repository constitution history and versioned in accordance with the change type. The maintainers are expected to verify that proposed changes remain aligned with training objectives, security discipline, and maintainability before approving them.

The project uses semantic versioning for constitutional changes:
- MAJOR: backward-incompatible governance or principle changes, or removal of non-negotiable rules
- MINOR: added or materially expanded principles, sections, or required process requirements
- PATCH: clarifying wording, editorial corrections, and non-semantic improvements

Compliance review expectations:
- Reviews must verify claims, access rules, and architectural decisions against this Constitution.
- Any exception that weakens security, user isolation, or documented training boundaries requires explicit approval and a recorded rationale.
- Changes that materially alter the application’s data boundaries, auth model, or architecture must include the corresponding update to documentation and verification evidence.

**Version**: 1.0.0 | **Ratified**: 2026-09-15 | **Last Amended**: 2026-09-15
