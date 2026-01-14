namespace TodoSeUsa.Application.Security;

public interface IRecoveryService
{
    Task<bool> UserHasRecoveryCode(string userId);
}