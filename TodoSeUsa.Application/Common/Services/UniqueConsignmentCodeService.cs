using TodoSeUsa.Application.Utilities;

namespace TodoSeUsa.Application.Common.Services;

public class UniqueConsignmentCodeService
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public UniqueConsignmentCodeService(IApplicationDbContextFactory dbContextFactory)
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
            code = CrockfordBase32CodeGenerator.CreateHyphenated(blockLength: 4);

            exists = await context.Consignments
                .AnyAsync(s => s.Code == code, ct);

        } while (exists);

        return code;
    }
}
