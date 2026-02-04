using System.Security.Cryptography;

namespace TodoSeUsa.Application.Security;

public static class RecoveryCodeGenerator
{
    private static readonly char[] AllowedChars = "23456789BCDFGHJKMNPQRTVWXY".ToCharArray();

    public static string Generate()
    {
        return string.Create(24, 0, static (buffer, _) =>
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = RandomChar();
            }
        });
    }

    private static char RandomChar()
    {
        var index = RandomNumberGenerator.GetInt32(AllowedChars.Length);
        return AllowedChars[index];
    }
}