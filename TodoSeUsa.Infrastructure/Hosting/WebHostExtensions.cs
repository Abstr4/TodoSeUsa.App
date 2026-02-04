using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace TodoSeUsa.Infrastructure.Hosting;

public static class WebHostExtensions
{
    public static void ConfigureProductionHosting(this WebApplicationBuilder builder, AppPaths paths)
    {
        if (builder.Environment.IsDevelopment())
            return;

        builder.WebHost.ConfigureHttps();
    }
}