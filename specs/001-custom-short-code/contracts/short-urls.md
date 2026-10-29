# Short URL API Contract

The feature extends the existing API; it adds no route.

## Create a short URL

`POST /urls`

### Request

```json
{
  "url": "https://balta.io",
  "code": "balta-io"
}
```

- `url` is required and must be an absolute HTTP or HTTPS URL.
- `code` is optional. When omitted or null, the service uses its existing automatic
  code generation behavior.
- When supplied, `code` must contain 3–32 ASCII letters, digits, hyphens, or
  underscores; `urls` and `openapi` are reserved without regard to capitalization.
- Code lookup and uniqueness are case-sensitive. A supplied empty or whitespace-only
  string is invalid.

### Responses

- **201 Created**: Returns the existing short URL response shape and uses the selected
  code in both the response and `Location` header.
- **400 Bad Request**: Validation problem for an invalid destination or custom code.
- **409 Conflict**: ProblemDetails explaining that the requested custom code is already
  in use. This applies both to a pre-existing code and a concurrent request that wins
  the unique-index race.

## List short URLs

`GET /urls`

Returns the existing list response shape. Custom-code and automatically generated
entries are included uniformly.

## Get short URL details

`GET /urls/{code}`

Returns the existing detail response for either kind of code, or the existing 404
behavior when no matching code exists.

## Follow a short URL

`GET /{code}`

Redirects to the saved original destination and increments the click count once for a
successful lookup, regardless of how the code was chosen. An unknown code retains the
existing 404 behavior.
