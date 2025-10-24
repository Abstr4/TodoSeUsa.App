namespace TodoSeUsa.Domain.Validators;

public static class NameValidator
{
    public static bool TryValidate(string input, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var cleaned = input.Trim();

        if (cleaned.Length < 2 || cleaned.Length > 50)
            return false;

        normalized = char.ToUpper(cleaned[0]) + cleaned[1..];

        return true;
    }
}