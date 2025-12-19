using System.Security.Cryptography;
using TodoSeUsa.Domain.Constants;

namespace TodoSeUsa.Application.Utilities;

public static class CrockfordBase32CodeGenerator
{
    private const string _alphabet = CrockfordBase32.Alphabet;

    public static string Create()
    {
        var chars = new char[10];
        for (int i = 0; i < 10; i++)
            chars[i] = _alphabet[RandomNumberGenerator.GetBytes(1)[0] % _alphabet.Length];

        return $"{new string(chars[..5])}-{new string(chars[5..])}";
    }
}
