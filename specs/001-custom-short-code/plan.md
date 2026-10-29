# Implementation Plan: Custom Short Codes

**Branch**: `001-custom-short-code` | **Date**: 2026-10-28 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-custom-short-code/spec.md`

## Summary

Extend the existing URL creation flow to accept an optional custom code. Keep the
current generated-code behavior when it is omitted. Validate custom codes in a small
service under `Services/`, preserve the existing unique database index, and return a
Portuguese 409 ProblemDetails response when a custom code loses a uniqueness race at
save time. Existing redirect, click counting, listing, and details remain unchanged.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core Minimal APIs, EF Core SQLite; no new packages

**Storage**: Existing SQLite database; `ShortUrls.Code` is already required, max 32
characters, and uniquely indexed. No schema change.

**Testing**: No test project. Add success and failure requests to
`src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http`; build the API with
`dotnet build`.

**Target Platform**: Existing ASP.NET Core API host

**Project Type**: Single web API project

**Performance Goals**: No new measurable latency target. Preserve the current creation,
listing, details, and redirect flows; validate format before database access and use an
existence check for early duplicate feedback.

**Constraints**: Preserve Minimal API + direct `AppDbContext` usage and `TypedResults`;
no new endpoint, migrations, NuGet dependencies, or test project. Custom codes are
3–32 ASCII letters/digits/hyphens/underscores; route names `urls` and `openapi` are
reserved without regard to capitalization. Existing code matching and uniqueness remain
case-sensitive.

**Scale/Scope**: One optional request field and its validation/conflict behavior in the
existing create endpoint; no schema or lifecycle changes.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Plan alignment |
|---|---|---|
| I. Simplicity | Pass (rechecked after design) | Keep the single API and direct endpoint-to-`AppDbContext` pattern. |
| II. Organization by Responsibility | Pass (rechecked after design) | Add a small custom-code rules service under `Services/`; update the existing contract and endpoint. |
| III. Explicit HTTP Contracts | Pass (rechecked after design) | Keep `Results<...>` and `TypedResults`; use 400 validation errors and 409 ProblemDetails for used codes. |
| IV. Persistence and Compatibility | Pass (rechecked after design) | Keep the existing unique index and schema; add `Code` as an optional request field. |
| V. Dependencies and Verification | Pass (rechecked after design) | Add no packages or test project; update `.http` cases and build without new warnings. |

No constitution violations or exceptions are required.

## Project Structure

### Documentation (this feature)

```text
specs/001-custom-short-code/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── short-urls.md
```

### Source Code

```text
src/EncurtaUrl/src/EncurtaUrl.Api/
├── Contracts/
│   └── ShortUrlContracts.cs       # Add optional Code request field
├── Endpoints/
│   └── ShortUrlEndpoints.cs       # Validate custom code and map duplicate conflicts
├── Services/
│   ├── ShortCodeGenerator.cs      # Preserve generator and its interface
│   └── CustomShortCodeRules.cs    # Format and reserved-name validation
└── EncurtaUrl.Api.http            # Add representative create/error/redirect cases
```

**Structure Decision**: Extend the existing single project using its established
`Contracts/`, `Endpoints/`, and `Services/` locations. Do not change `Models/`,
`Data/AppDbContext.cs`, schema, or route layout.

## Complexity Tracking

No constitution violations. No additional complexity requires justification.
