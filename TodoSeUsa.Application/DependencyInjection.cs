using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Boxes.Interfaces;
using TodoSeUsa.Application.Features.Boxes.Services;
using TodoSeUsa.Application.Features.Consignments.Interfaces;
using TodoSeUsa.Application.Features.Consignments.Services;

using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Services;
using TodoSeUsa.Application.Features.Providers.Interfaces;
using TodoSeUsa.Application.Features.Providers.Services;

namespace TodoSeUsa.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IBoxService, BoxService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IConsignmentService, ConsignmentService>();
        builder.Services.AddScoped<IProviderService, ProviderService>();
        builder.Services.AddScoped<IPersonService, PersonService>();
        builder.Services.AddScoped<IProviderService, ProviderService>();
    }
}