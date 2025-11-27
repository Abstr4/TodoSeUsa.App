using TodoSeUsa.Application.Common.Interfaces;

namespace TodoSeUsa.Infrastructure.Data;

public class ApplicationDbContextFactory : IApplicationDbContextFactory
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public ApplicationDbContextFactory(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<IApplicationDbContext> CreateDbContextAsync(CancellationToken ct = default)
    {
        return await _factory.CreateDbContextAsync(ct);
    }
}