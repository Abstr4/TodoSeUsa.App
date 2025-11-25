using System.Net.Mail;
using System.Text.RegularExpressions;
using TodoSeUsa.Domain.Validators;

namespace TodoSeUsa.BlazorServer.Helpers;

public static class InputValidators
{
    public static bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber)) return true;

        return DomainValidators.PhoneValidator(phoneNumber);
    }

    public static bool IsValidEmailAddress(string? emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress)) return true;

        return DomainValidators.EmailValidator(emailAddress);
    }

    public static bool IsValidContactInformation(string? emailAddress, string? phoneNumber)
    {
        return !string.IsNullOrEmpty(emailAddress) || !string.IsNullOrEmpty(phoneNumber);
    }

    public static bool IsNullOrEmptyOrMatchesAlphanumericPattern(string? input)
    {
        if (string.IsNullOrEmpty(input)) return true;

        return DomainValidators.ContainsAlphanumeric(input);
    }
}
