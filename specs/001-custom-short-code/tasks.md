# Tasks: Custom Short Codes

**Input**: Design documents from `/specs/001-custom-short-code/`

**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/](./contracts/), [quickstart.md](./quickstart.md)

**Tests**: No test project or test suite is requested. Add manual success and error requests to the existing `.http` file and verify the build.

**Organization**: Tasks are grouped by user story for traceability. The stories share the existing create flow, so implementation is incremental.

## Phase 1: Setup

**Purpose**: Verify the existing project baseline before making changes.

- [X] T001 Run `dotnet build` for `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.csproj`; recorded baseline failure `NU1301` because the configured NuGet source is unreachable.

---

## Phase 2: Foundational

**Purpose**: Establish prerequisites that block story work.

No separate foundational changes are needed: the API project, SQLite context, unique code index, and error middleware already exist.

**Checkpoint**: Proceed to user stories after confirming the baseline build.

---

## Phase 3: User Story 1 - Create a short URL with a custom code (Priority: P1) 🎯 MVP

**Goal**: Allow users to choose a valid memorable code while preserving automatic generation when no code is supplied.

**Independent Test**: Create a short URL with a valid custom code and verify the response uses it, a request without a code still generates one, and opening the custom link redirects and increments clicks.

### Implementation for User Story 1

- [X] T002 [P] [US1] Add `ICustomShortCodeRules` and its implementation in `src/EncurtaUrl/src/EncurtaUrl.Api/Services/CustomShortCodeRules.cs`; validate custom codes as 3–32 ASCII letters/digits/hyphens/underscores, rejecting route names `urls` and `openapi` without regard to capitalization.
- [X] T003 [P] [US1] Extend `CreateShortUrlRequest` with an optional nullable `Code` property defaulting to null in `src/EncurtaUrl/src/EncurtaUrl.Api/Contracts/ShortUrlContracts.cs`.
- [X] T004 [US1] Register `ICustomShortCodeRules` in `src/EncurtaUrl/src/EncurtaUrl.Api/Program.cs` without adding a package or changing other service registrations.
- [X] T005 [US1] Update `CreateAsync` in `src/EncurtaUrl/src/EncurtaUrl.Api/Endpoints/ShortUrlEndpoints.cs` to preserve URL validation, reject invalid supplied codes with `ValidationProblem`, persist a valid supplied code, and use the existing generator only when `Code` is omitted or null; leave generated-code behavior unchanged.
- [X] T006 [P] [US1] Add `POST /urls` scenarios with `code` omitted and with `"code": null` (both must use automatic generation), valid custom codes of 3 and 32 characters, and `balta-io`; then verify `GET /balta-io` redirects and `GET /urls/balta-io` reports the incremented click count in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http`.

**Checkpoint**: Custom-code creation, automatic generation, redirect, and click counting are independently verifiable.

---

## Phase 4: User Story 2 - Receive clear feedback for an unusable code (Priority: P1)

**Goal**: Reject malformed, reserved, and duplicate codes without creating a short URL, including when concurrent requests race.

**Independent Test**: Submit invalid, reserved, and already-used codes; verify a 400 validation problem or 409 conflict as appropriate and confirm no duplicate record is created.

### Implementation for User Story 2

- [X] T007 [US2] Update `CreateAsync` in `src/EncurtaUrl/src/EncurtaUrl.Api/Endpoints/ShortUrlEndpoints.cs` to return a Portuguese 409 ProblemDetails when a custom code already exists and, around `SaveChangesAsync`, map only `DbUpdateException` with inner SQLite `SqliteExtendedErrorCode == 2067` to 409 after confirming the requested code now exists; rethrow unrelated database failures.
- [X] T008 [US2] Add `POST /urls` error scenarios for 2-character and 33-character codes, disallowed characters, empty and whitespace-only codes, case-insensitive reserved code `URLs`, and reusing `balta-io`; assert invalid-code requests return 400 without creating a short URL and the duplicate returns 409 in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http`.

**Checkpoint**: Existing duplicates and unique-index races produce the same clear code-in-use conflict; unrelated save errors are not disguised as conflicts.

---

## Phase 5: User Story 3 - Continue managing and using short URLs (Priority: P2)

**Goal**: Ensure custom-code links remain compatible with existing listing, details, and redirect journeys.

**Independent Test**: List short URLs and request details for `balta-io`; verify the same response fields as generated-code links and a click count reflecting each successful redirect.

### Implementation for User Story 3

- [X] T009 [US3] Add `GET /urls` and `GET /urls/balta-io` verification requests after creating the custom URL in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http`; retain the existing response fields and not-found request for unknown codes.

**Checkpoint**: Generated and custom codes share the existing list, details, and navigation behavior.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Update user-facing documentation and verify the completed feature.

- [X] T010 [P] Update `src/EncurtaUrl/README.md` to document the optional custom code, its allowed format, reserved route names, and 400/409 outcomes without changing the existing endpoint set.
- [ ] T011 Run `dotnet build` for `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.csproj` and execute the scenarios in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http` and `specs/001-custom-short-code/quickstart.md`; confirm no new build warnings, the custom-code workflows succeed, and the concurrent duplicate scenario has at most one success.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No code changes; the baseline build must be checked first.
- **Foundational (Phase 2)**: No additional work; existing infrastructure satisfies prerequisites.
- **User Stories (Phase 3+)**: Begin after the baseline build. The implementation is cumulative because the stories extend the same create request and endpoint.
- **Polish (Phase 6)**: Depends on the desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after the baseline build. It establishes optional custom-code creation and validation.
- **User Story 2 (P1)**: Depends on User Story 1 because duplicate and race handling require the custom-code create path.
- **User Story 3 (P2)**: Depends on User Story 1 to create custom-code records; listing and detail behavior reuse existing handlers.
- **User Story 3 verification (T009)** follows User Story 2's `.http` updates to avoid concurrent edits to the same file.

### Within Each User Story

- The custom-code rules and request contract can be implemented in parallel (T002, T003).
- Service registration follows creation of the rules service; endpoint integration follows the contract and registration.
- HTTP requests can be authored independently of source files, then executed after implementation.
- No schema, model, migration, package, or standalone test-project tasks are required.

### Parallel Opportunities

- T002 and T003 in User Story 1 modify different files and have no prerequisite on each other.
- T006 can be authored in parallel with T002 and T003 because it edits only the HTTP scenario file.
- T010 documentation can be updated independently of application code after the feature behavior is established.
- T007 and T008 touch different files and can be prepared in parallel after User Story 1 is complete; T009 is serialized after T008 because both edit the same `.http` file.

---

## Parallel Example: User Story 1

```text
Task: T002 Add custom-code rules in src/EncurtaUrl/src/EncurtaUrl.Api/Services/CustomShortCodeRules.cs
Task: T003 Add the optional request code in src/EncurtaUrl/src/EncurtaUrl.Api/Contracts/ShortUrlContracts.cs
Task: T006 Add the creation and redirect scenarios in src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http
```

## Implementation Strategy

### MVP First (User Story 1)

1. Verify the baseline with T001.
2. Complete User Story 1 to support valid custom codes and preserve automatic generation.
3. Validate its creation, redirect, and click-count scenarios independently.
4. Continue to User Story 2 before considering the feature production-ready, so invalid and duplicate codes are safely rejected.

### Incremental Delivery

1. Complete User Story 1 and verify creation compatibility.
2. Complete User Story 2 and verify validation, conflict, and race handling.
3. Complete User Story 3 and verify list/details compatibility.
4. Update documentation and run the complete quickstart and build.
