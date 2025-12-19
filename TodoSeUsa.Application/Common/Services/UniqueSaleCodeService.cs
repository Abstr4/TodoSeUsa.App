using TodoSeUsa.Application.Utilities;

namespace TodoSeUsa.Application.Common.Services;

public class UniqueSaleCodeService
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public UniqueSaleCodeService(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<string> GenerateAsync(CancellationToken ct = default)
    {

        var context = await _dbContextFactory.CreateDbContextAsync(ct);

        string code;
        bool exists;

        do
        {
            code = CrockfordBase32CodeGenerator.Create();

            exists = await context.Sales
                .AnyAsync(s => s.Code == code, ct);

        } while (exists);

        return code;
    }
}
