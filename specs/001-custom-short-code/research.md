# Research: Custom Short Codes

## Decisions

### Optional custom code in the existing create flow

- **Decision**: Add an optional `Code` property to the existing create request. If omitted
  or null, retain automatic generation. Reject a supplied blank or malformed code rather
  than trimming it or silently generating a replacement.
- **Rationale**: This preserves existing clients and matches the explicit distinction
  between an omitted code and a code the user intentionally supplied.
- **Alternatives considered**: A separate creation endpoint or silently falling back to
  generated codes for invalid input. Both would make the flow inconsistent or hide user
  errors.

### Custom code validation and reserved route names

- **Decision**: Keep format, length, and reserved-name rules in a small service under
  `Services/`. Accept only ASCII letters, digits, hyphens, and underscores, with lengths
  3–32 inclusive. Reject `urls` and `openapi` case-insensitively.
- **Rationale**: This keeps validation reusable and separates it from endpoint
  orchestration without adding a validation package. Other code comparisons and database
  uniqueness remain case-sensitive.
- **Alternatives considered**: Inline all validation in the endpoint or introduce an
  external validator dependency; neither is needed for this small rule set.

### SQLite unique-constraint race handling

- **Decision**: Keep the existing unique index on `ShortUrls.Code` and perform an
  existence check for early duplicate feedback. On `SaveChangesAsync`, catch
  `DbUpdateException` only when its inner `SqliteException` has
  `SqliteExtendedErrorCode == 2067` (`SQLITE_CONSTRAINT_UNIQUE`). Confirm that the
  requested code now exists using the same case-sensitive equality semantics; return
  409 only in that case, and rethrow otherwise.
- **Rationale**: A pre-save query alone cannot prevent two concurrent requests from
  passing the check. SQLite's primary error code 19 covers multiple constraint failures,
  while extended code 2067 identifies a unique constraint. The extended code alone does
  not identify the index, so confirming the requested code avoids presenting an unrelated
  unique-constraint failure as a duplicate-code response.
- **Alternatives considered**: Rely on the pre-save check (race-prone); map every
  `DbUpdateException` or primary code 19 to 409 (masks unrelated errors); parse the
  provider-generated exception message (brittle and not a structured index identifier).
- **Sources**:
  - [Microsoft.Data.Sqlite `SqliteException`](https://learn.microsoft.com/dotnet/api/microsoft.data.sqlite.sqliteexception?view=msdata-sqlite-10.0.0)
  - [SQLite result codes](https://www.sqlite.org/rescode.html)
  - [Microsoft.Data.Sqlite database errors](https://learn.microsoft.com/dotnet/standard/data/sqlite/database-errors)
  - [EF Core `DbUpdateException`](https://learn.microsoft.com/dotnet/api/microsoft.entityframeworkcore.dbupdateexception?view=efcore-10.0)

### Persistence, HTTP behavior, and verification

- **Decision**: Do not change the schema, add routes, NuGet packages, or a test project.
  Keep `TypedResults` responses, report invalid inputs as 400 validation problems and
  duplicate codes as 409 ProblemDetails, and extend `EncurtaUrl.Api.http`.
- **Rationale**: The existing entity already stores a required 32-character code with a
  unique index. Reusing the current create, lookup, list, and redirect flows preserves
  behavior and adheres to project conventions.
- **Alternatives considered**: Schema migrations, a new endpoint, a new validation
  dependency, or a separate test project; none is needed for the requested behavior.
