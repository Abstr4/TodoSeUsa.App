using System.Text.RegularExpressions;

namespace TodoSeUsa.Domain.Validators;

public static class PhoneNumberValidator
{
    public static bool TryValidate(string? input, out string? normalized)
    {
        normalized = null;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var cleaned = Normalize(input);
        if (!IsValidFormatE164(cleaned))
            return false;

        normalized = cleaned;
        return true;
    }

    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Phone number can't be empty.");

        return Regex.Replace(input.Trim(), @"(?!^\+)\D", "");
    }

    public static bool IsValidFormatE164(string normalized)
    {
        return Regex.IsMatch(normalized, @"^\+[1-9]\d{7,14}$");
    }
}