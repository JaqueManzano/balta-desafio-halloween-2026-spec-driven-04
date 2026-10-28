<!--
Sync Impact Report
Version change: unfilled scaffold → 1.0.0 (initial ratification of project conventions)
Modified principles: unfilled scaffold → I. Simplicity; II. Organization by Responsibility;
  III. Explicit HTTP Contracts; IV. Persistence and Compatibility; V. Dependencies and Verification
Added sections: Technology and Quality Constraints; Development Workflow
Removed sections: none
Follow-up TODOs: confirm the original ratification date.
-->

# EncurtaUrl Constitution

## Core Principles

### I. Simplicity
The API MUST remain a single .NET 10 Minimal API. Endpoints MUST use `AppDbContext`
directly; the project MUST NOT add layers, MediatR, or repositories without an approved
constitution amendment. This keeps the existing application small and consistent.

### II. Organization by Responsibility
Endpoint mappings and handlers MUST live in `Endpoints/` as extensions over
`IEndpointRouteBuilder`, using route groups where appropriate. Request and response
records MUST live in `Contracts/`, persisted entities in `Models/`, and small services
with interfaces in `Services/`. This structure MUST be reused rather than introducing
parallel conventions.

### III. Explicit HTTP Contracts
Endpoints MUST declare their outcomes with `TypedResults` and `Results<...>` signatures.
Input validation failures MUST return `ValidationProblem` with HTTP 400. Other failures
MUST return `ProblemDetails` with the appropriate status code. User-facing error messages
MUST be written in Portuguese.

### IV. Persistence and Compatibility
The application MUST use EF Core with SQLite and create its database with
`EnsureCreated`; it MUST NOT introduce migrations. Any schema change MUST be justified in
the feature plan. Existing request and response contracts MUST remain compatible; new
request fields MUST be optional unless a deliberate breaking change is approved.

### V. Dependencies and Verification
Features MUST NOT add NuGet packages without explicit justification in the plan. Because
the project has no test project, each feature MUST add representative success and error
scenarios to `EncurtaUrl.Api.http`. The API project MUST compile with `dotnet build`
without introducing new warnings.

## Technology and Quality Constraints

- The API targets .NET 10 and uses Minimal APIs.
- Persistence uses EF Core and SQLite; the current database is initialized without
  migrations.
- OpenAPI is available in Development, and the application uses ProblemDetails and
  standard exception and status-code handling middleware.
- Changes MUST preserve established behavior unless the feature explicitly specifies a
  user-visible change.

## Development Workflow

- Before implementation, inspect the existing API and its established patterns.
- Feature specifications MUST describe user outcomes and testable requirements; plans
  MUST explain deviations from this constitution, including schema or dependency changes.
- Update directly related documentation when behavior or usage changes.
- Validate changes using the repository's available checks, including the HTTP scenarios
  and a warning-clean build required by the relevant feature.

## Governance

This constitution governs feature specifications, plans, implementation, and review.
Proposed changes to its principles or constraints MUST be documented and reviewed before
implementation; a plan MUST identify any needed exception and its rationale. Reviews
MUST verify compliance and require deviations to be corrected or explicitly approved.

Constitution versions follow semantic versioning: MAJOR for incompatible governance
changes, MINOR for new principles or materially expanded constraints, and PATCH for
clarifications or non-semantic refinements. Every amendment MUST update the version,
last-amended date, and a sync impact report; the report is temporary and MUST be removed
before the amended file is committed.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): confirm original adoption date | **Last Amended**: 2026-10-28
