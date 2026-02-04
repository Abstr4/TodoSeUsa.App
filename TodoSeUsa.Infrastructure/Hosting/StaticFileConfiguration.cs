using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace TodoSeUsa.Infrastructure.Hosting;

public static class StaticFileConfiguration
{
    public static void UseStorageFiles(this WebApplication app, string storagePath
    )
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(storagePath),
            RequestPath = "/files"
        });
    }
}