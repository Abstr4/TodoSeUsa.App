namespace TodoSeUsa.Domain.Validators;

public static class AddressValidator
{
    public static bool TryValidate(string? address, out string? normalized)
    {
        normalized = null;
        if (string.IsNullOrWhiteSpace(address) || address.Trim().Length <= 2)
            return false;

        var trimmed = address.Trim();

        bool containsNumber = trimmed.Any(char.IsDigit);
        if (!containsNumber)
            return false;

        normalized = trimmed;
        return true;
    }
}