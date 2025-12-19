using System.Text.RegularExpressions;
using TodoSeUsa.Domain.Entities;

namespace TodoSeUsa.Domain.Validators;

public static class DomainValidators
{

    public static bool PhoneValidator(string phone)
    {
        /* Matches:
                    123
                    123 456
                    123-456
                    1-2-3
                    12 34-56 78
                    000-000 000
        */
        return Regex.IsMatch(phone, @"^[0-9]+([ -][0-9]+)*$");
    }


    public static bool EmailValidator(string email)
    {
        /* Matches:
                    a@b
                    abc@xyz
                    hello@world.com
                    user.name@domain
                    user+tag@domain
                    u@d
         */
        int index = email.IndexOf('@');

        return
            index > 0 &&
            index != email.Length - 1 &&
            index == email.LastIndexOf('@');
    }

    public static bool HasAtLeastOneContactMethod(Person person)
    {
        return HasValue(person.EmailAddress) ||
               HasValue(person.PhoneNumber);
    }

    public static bool HasValue(string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static bool ContainsAlphanumeric(string input)
    {
        return Regex.IsMatch(input, @"^(?=.*[A-Za-z0-9]).+$");
    }
}