# Feature Specification: Custom Short Codes

**Feature Branch**: `not-created`

**Created**: 2026-10-28

**Status**: Draft

**Input**: User description: "Permitir que o usuário informe um código customizado ao encurtar uma URL. O código é opcional; códigos válidos precisam ser únicos e não conflitar com rotas da API. Listagem, detalhes e redirecionamento devem continuar funcionando. Fora do escopo: editar ou remover código, autenticação e expiração."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create a short URL with a custom code (Priority: P1)

As a person creating a short URL, I want to choose a memorable code so that the resulting link is easier to share and recognize.

**Why this priority**: Choosing a memorable link is the central value of this feature.

**Independent Test**: Submit a valid destination and custom code, then confirm the returned short link contains that code and opens the destination.

**Acceptance Scenarios**:

1. **Given** a valid destination and an unused custom code, **When** the person creates a short URL, **Then** the short link uses the requested code.
2. **Given** a valid destination and no custom code, **When** the person creates a short URL, **Then** the system generates a code as it does today.
3. **Given** a short URL created with a custom code, **When** a visitor opens that code, **Then** the visitor is redirected to the original destination and the click count increases by one.

### User Story 2 - Receive clear feedback for an unusable code (Priority: P1)

As a person choosing a code, I want a clear explanation when the code cannot be used so that I can correct it or choose another one.

**Why this priority**: Invalid or already claimed codes must not produce misleading links or silently change the code.

**Independent Test**: Attempt creation with malformed, reserved, and already-used codes, and verify each is rejected with a clear error and no short URL is created.

**Acceptance Scenarios**:

1. **Given** a code containing a character outside the allowed set or outside the permitted length, **When** the person creates a short URL, **Then** the request is rejected as invalid.
2. **Given** a code reserved for an existing API route, **When** the person creates a short URL, **Then** the request is rejected as invalid.
3. **Given** a code already assigned to another short URL, **When** the person creates a short URL, **Then** creation is rejected and the response says that the code is already in use.

### User Story 3 - Continue managing and using short URLs (Priority: P2)

As a person using the existing short URL service, I want custom-code links to behave like generated-code links so that I can find their details and use them without a separate workflow.

**Why this priority**: Custom codes are only useful if they integrate with the service's existing list, details, and redirect journeys.

**Independent Test**: Create a custom-code URL and verify it appears in the list, can be retrieved by code, and redirects with click tracking.

**Acceptance Scenarios**:

1. **Given** a custom-code short URL, **When** a person lists short URLs, **Then** it appears with its code and existing URL details.
2. **Given** a custom-code short URL, **When** a person requests its details by code, **Then** the same detail fields available for generated codes are returned.

### Edge Cases

- Codes at the minimum and maximum permitted lengths are accepted; values one character below or above those limits are rejected.
- A whitespace-only or empty custom code is treated as invalid when supplied; an omitted code requests automatic generation.
- When two creation requests attempt the same custom code at nearly the same time, no more than one succeeds and the other receives an already-in-use error.
- Reserved codes are rejected even when they otherwise satisfy the character and length rules.
- A code that does not exist continues to follow the current not-found behavior when opened or queried.
- An invalid destination URL continues to be rejected under the existing creation rules, regardless of whether a custom code was supplied.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST allow a person to optionally provide a custom code when creating a short URL.
- **FR-002**: When no custom code is provided, the system MUST continue generating codes according to the existing behavior.
- **FR-003**: The system MUST accept custom codes containing only ASCII letters, digits, hyphens, and underscores.
- **FR-004**: A custom code MUST contain between 3 and 32 characters, inclusive.
- **FR-005**: The system MUST reject custom codes that conflict with existing API route names, including `urls` and `openapi`.
- **FR-006**: The system MUST ensure each code identifies no more than one short URL. If a custom code is already in use, creation MUST be rejected with a clear conflict response indicating that the code is in use.
- **FR-007**: Concurrent attempts to create a short URL with the same custom code MUST result in at most one successful creation; each unsuccessful attempt MUST receive the same code-in-use outcome.
- **FR-008**: A successfully created custom-code URL MUST be returned with the chosen code in its short link.
- **FR-009**: Opening a custom-code short link MUST redirect to its original destination and increment the click count by one, matching generated-code behavior.
- **FR-010**: Custom-code URLs MUST appear in listing and detail results with the same fields and behavior as generated-code URLs.
- **FR-011**: Supplying an invalid custom code MUST reject creation without creating a short URL and MUST explain the validation error.
- **FR-012**: This feature MUST NOT provide code editing or removal, authentication, or link expiration.

### Key Entities

- **Short URL**: A saved association between an original destination and a unique code, with its creation time and click count. The code may be generated or chosen by the person creating the link.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All valid custom codes within the defined character and length limits can be used to create short URLs.
- **SC-002**: 100% of tested invalid, reserved, or already-used custom codes are rejected without creating a short URL and return an actionable error.
- **SC-003**: 100% of tested custom-code redirects reach the saved original destination and increase its click count exactly once per redirect.
- **SC-004**: Existing creation requests that omit a custom code continue to succeed using automatic code generation.
- **SC-005**: Custom-code links appear in the existing listing and details with all existing response information.

## Assumptions

- The existing service's code storage limit of 32 characters is the upper bound for custom codes; a minimum of 3 characters provides a practical baseline for memorable aliases.
- Code matching is case-sensitive, consistent with path-based codes; differently cased values are distinct codes.
- `urls` and `openapi` are reserved because those names are used by existing API routes. Additional route names must also be reserved if introduced.
- Validation errors use the existing validation response convention, while code uniqueness conflicts use a conflict response.
- The current short URL creation, listing, details, and redirect behavior remains the source of truth for all fields and responses not changed by this feature.
