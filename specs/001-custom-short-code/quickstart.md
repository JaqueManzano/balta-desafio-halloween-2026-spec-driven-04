# Quickstart: Custom Short Codes

## Prerequisites

- .NET 10 SDK
- Run the API from `src/EncurtaUrl/src/EncurtaUrl.Api`
- Default local HTTP base URL: `http://localhost:5080`

## Build

From the API project directory:

```powershell
dotnet build
```

The build should complete without new warnings.

## End-to-end scenarios

Use the corresponding requests in `src/EncurtaUrl/src/EncurtaUrl.Api/EncurtaUrl.Api.http`.

1. **Generated code remains compatible**: Submit only `url` to `POST /urls`. Expect
   `201 Created` and an automatically generated code.
2. **Custom code succeeds**: Submit `url` and `"code": "balta-io"`. Expect `201 Created`,
   with `balta-io` in the response and short-link URL.
3. **Redirect and click count**: Open `GET /balta-io`. Expect a redirect to
   `https://balta.io`; request `GET /urls/balta-io` and confirm the click count increased
   by one.
4. **Listing and details**: Request `GET /urls` and `GET /urls/balta-io`. Expect the
   custom-code entry and the same fields as generated-code entries.
5. **Invalid input**: Try a two-character code, a code with punctuation outside the
   allowed set, and a reserved name such as `URLs`. Expect `400` and no created record.
6. **Duplicate code**: Submit the same custom code again. Expect `409 Conflict` with an
   explanation that the code is already in use.
7. **Concurrent duplicate**: Send two create requests with the same unused custom code
   concurrently. Expect at most one `201`; the other request must receive `409`.

To send a concurrent duplicate check from PowerShell while the API is running, use a
fresh code value and run:

```powershell
$client = [System.Net.Http.HttpClient]::new()
$uri = 'http://localhost:5080/urls'
$json = '{"url":"https://balta.io","code":"race-check-01"}'
$send = {
    param($httpClient, $target, $body)
    $content = [System.Net.Http.StringContent]::new(
        $body,
        [System.Text.Encoding]::UTF8,
        'application/json')
    $httpClient.PostAsync($target, $content)
}
$first = $send.Invoke($client, $uri, $json)
$second = $send.Invoke($client, $uri, $json)
[System.Threading.Tasks.Task]::WaitAll(@($first, $second))
@($first.Result.StatusCode, $second.Result.StatusCode)
$client.Dispose()
```

The two results must be one `Created` (201) and one `Conflict` (409).

## Contract and model references

- Request and route behavior: [contracts/short-urls.md](./contracts/short-urls.md)
- Stored data and validation constraints: [data-model.md](./data-model.md)
