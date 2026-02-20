using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TodoSeUsa.Application.Common.Services.People;
using TodoSeUsa.Application.Features.Boxes.Interfaces;
using TodoSeUsa.Application.Features.Boxes.Services;
using TodoSeUsa.Application.Features.Consignments.Interfaces;
using TodoSeUsa.Application.Features.Consignments.Services;
using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Services;
using TodoSeUsa.Application.Features.Consignors.Interfaces;
using TodoSeUsa.Application.Features.Consignors.Services;
using TodoSeUsa.Application.Features.Sales.Interfaces;
using TodoSeUsa.Application.Features.Sales.Services;
using TodoSeUsa.Application.Security;
using TodoSeUsa.Application.Common.Events;
using TodoSeUsa.Application.Features.Payouts.Interfaces;
using TodoSeUsa.Application.Features.Payouts.Services;
using TodoSeUsa.Application.Features.Overview;

namespace TodoSeUsa.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IOverviewService, OverviewService>();
        builder.Services.AddScoped<IBoxService, BoxService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IConsignmentService, ConsignmentService>();
        builder.Services.AddScoped<IConsignorService, ConsignorService>();
        builder.Services.AddScoped<IPersonService, PersonService>();
        builder.Services.AddScoped<ISaleService, SaleService>();
        builder.Services.AddScoped<IPayoutService, PayoutService>();

        builder.Services.AddScoped<IRecoveryCodeHasher, RecoveryCodeHasher>();
        builder.Services.AddScoped<UniqueSaleCodeService>();
        builder.Services.AddScoped<UniqueConsignmentCodeService>();
        builder.Services.AddScoped<IProductImageService, ProductImageService>();


        builder.Services.AddSingleton<AppEvents>();
    }
}