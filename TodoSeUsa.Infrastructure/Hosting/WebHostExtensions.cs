using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoSeUsa.Infrastructure.Security;

namespace TodoSeUsa.Infrastructure.Hosting;

public static class WebHostExtensions
{
    private const string _kestrelCertPassword = "TodoSeUsa-Kestrel-Cert";

    public static void ConfigureProductionHosting(
        this WebApplicationBuilder builder,
        AppPaths paths)
    {
        if (builder.Environment.IsDevelopment())
            return;

        KestrelCertificateService.EnsureCertificate(
            paths.Certificate,
            _kestrelCertPassword);

        builder.WebHost.ConfigureHttps(
            paths.Certificate,
            _kestrelCertPassword);
    }
}