namespace EncurtaUrl.Api.Services;

public interface ICustomShortCodeRules
{
    string? Validate(string code);
}

public sealed class CustomShortCodeRules : ICustomShortCodeRules
{
    private static readonly HashSet<string> ReservedCodes =
        new(StringComparer.OrdinalIgnoreCase) { "urls", "openapi" };

    public string? Validate(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return "Informe um código personalizado.";

        if (code.Length is < 3 or > 32)
            return "O código deve ter entre 3 e 32 caracteres.";

        if (ReservedCodes.Contains(code))
            return "Este código é reservado para uma rota da API.";

        if (code.Any(character =>
                !char.IsAsciiLetterOrDigit(character) &&
                character is not '-' and not '_'))
            return "Use apenas letras, números, hífen e underscore no código.";

        return null;
    }
}
