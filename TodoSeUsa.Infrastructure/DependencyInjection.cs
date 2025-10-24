using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoSeUsa.Application.Common.Interfaces;
using TodoSeUsa.Infrastructure.Data.Interceptors;

namespace TodoSeUsa.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        const string connectionStringName = "DefaultConnection";
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
        Guard.Against.Null(connectionString, message: $"Connection string '{connectionStringName}' not found.");

        builder.Services.AddScoped<SoftDeleteInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<SoftDeleteInterceptor>();

            options
                .UseSqlServer(connectionString)
                .AddInterceptors(interceptor);
        });

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}