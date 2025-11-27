namespace TodoSeUsa.Application.Common.Interfaces;

public interface IApplicationDbContextFactory
{
    Task<IApplicationDbContext> CreateDbContextAsync(CancellationToken ct = default);
}