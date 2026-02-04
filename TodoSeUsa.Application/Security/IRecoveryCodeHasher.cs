namespace TodoSeUsa.Application.Security;

public interface IRecoveryCodeHasher
{
    public string Hash(string password);

    public bool Verify(string code, string storedHash);
}