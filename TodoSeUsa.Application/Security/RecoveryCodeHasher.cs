using System.Security.Cryptography;
using System.Text;

namespace TodoSeUsa.Application.Security;

public sealed class RecoveryCodeHasher : IRecoveryCodeHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100000;

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public string Hash(string code)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(code, salt, Iterations, Algorithm, KeySize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string code, string storedHash)
    {
        string[] parts = storedHash.Split('-');
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(code, salt, Iterations, Algorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}
