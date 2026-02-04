using System.Text.RegularExpressions;

namespace TodoSeUsa.Domain.Validators;

public static class CodesValidators
{
    public static bool ValidateBoxCode(string code)
    {
        return Regex.IsMatch(code, @"^BOX-\d+$");
    }

    public static bool ValidateProductCode(string code)
    {
        return Regex.IsMatch(code, @"^TSU-\d+$");
    }
}