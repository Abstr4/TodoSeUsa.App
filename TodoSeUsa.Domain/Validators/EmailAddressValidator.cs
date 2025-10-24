using System.ComponentModel.DataAnnotations;

namespace TodoSeUsa.Domain.Validators;

public static class EmailAddressValidator
{
    public static bool TryValidate(string? input, out string? normalized)
    {
        normalized = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var cleaned = input.Trim().ToLowerInvariant();
        var validator = new EmailAddressAttribute();
        if (!validator.IsValid(cleaned))
            return false;

        normalized = cleaned;
        return true;
    }
}