# EncurtaUrl

API de encurtamento de URLs com .NET 10, Minimal APIs, EF Core e SQLite.

## Como rodar

```bash
cd src/EncurtaUrl.Api
dotnet run
```

A API sobe em `http://localhost:5080` e o banco `encurtaurl.db` é criado na primeira execução.

## Endpoints

| Método | Rota | Descrição |
| --- | --- | --- |
| POST | `/urls` | Cria uma URL curta com código gerado ou código customizado opcional |
| GET | `/urls` | Lista todas as URLs curtas |
| GET | `/urls/{code}` | Detalhes de uma URL curta |
| GET | `/{code}` | Redireciona (302) para a URL original e conta o clique |
| GET | `/openapi/v1.json` | Documento OpenAPI (apenas em Development) |

Use o arquivo `src/EncurtaUrl.Api/EncurtaUrl.Api.http` para testar.

Para escolher um código, envie `code` junto com `url` no `POST /urls`:

```json
{
  "url": "https://balta.io",
  "code": "balta-io"
}
```

O código customizado aceita de 3 a 32 letras ASCII, números, hífen e underscore.
Os nomes de rota `urls` e `openapi` são reservados, sem distinção entre maiúsculas
e minúsculas. Se `code` for omitido ou `null`, o código continua sendo gerado
automaticamente. Um código inválido retorna `400`; um código já utilizado retorna `409`.

## Estrutura

```
src/EncurtaUrl.Api
├── Contracts/ShortUrlContracts.cs   # request e response
├── Data/AppDbContext.cs             # EF Core + índice único em Code
├── Endpoints/ShortUrlEndpoints.cs   # Minimal APIs
├── Models/ShortUrl.cs               # entidade
├── Services/CustomShortCodeRules.cs # validação de códigos customizados
├── Services/ShortCodeGenerator.cs   # código aleatório base62 de 7 caracteres
└── Program.cs
```
