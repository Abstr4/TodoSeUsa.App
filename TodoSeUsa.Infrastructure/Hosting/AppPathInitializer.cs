using Microsoft.AspNetCore.Builder;

namespace TodoSeUsa.Infrastructure.Hosting;

public sealed record AppPaths( string AppData, string Storage, string Certificate);

public static class AppPathInitializer
{
    public static AppPaths Initialize(WebApplicationBuilder builder)
    {
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TodoSeUsa"
        );

        var storage = Path.Combine(
            builder.Environment.ContentRootPath,
            builder.Configuration["Storage:BasePath"] ?? "Storage"
        );

        var cert = Path.Combine(appData, "kestrel.pfx");

        Directory.CreateDirectory(appData);
        Directory.CreateDirectory(storage);

        return new AppPaths(appData, storage, cert);
    }
}
