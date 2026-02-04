using Microsoft.AspNetCore.Hosting;

namespace TodoSeUsa.Infrastructure.Hosting;

public static class KestrelExtensions
{
    public static void ConfigureHttps(this IWebHostBuilder host, string certPath, string password)
    {
        host.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5001, listen =>
            {
                listen.UseHttps(certPath, password);
            });
        });
    }
}
