using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TodoSeUsa.Application.Features.Boxes.Interfaces;
using TodoSeUsa.Application.Features.Boxes.Services;
using TodoSeUsa.Application.Features.Clients.Interfaces;
using TodoSeUsa.Application.Features.Clients.Services;
using TodoSeUsa.Application.Features.Consignments.Interfaces;
using TodoSeUsa.Application.Features.Consignments.Services;
using TodoSeUsa.Application.Features.LoanNotes.Interfaces;
using TodoSeUsa.Application.Features.LoanNotes.Services;
using TodoSeUsa.Application.Features.Payments.Interfaces;
using TodoSeUsa.Application.Features.Payments.Services;
using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Services;
using TodoSeUsa.Application.Features.Providers.Interfaces;
using TodoSeUsa.Application.Features.Providers.Services;
using TodoSeUsa.Application.Features.Reservations.Interfaces;
using TodoSeUsa.Application.Features.Reservations.Services;
using TodoSeUsa.Application.Features.Sales.Interfaces;
using TodoSeUsa.Application.Features.Sales.Services;

namespace TodoSeUsa.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IBoxService, BoxService>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IConsignmentService, ConsignmentService>();
        builder.Services.AddScoped<IProviderService, ProviderService>();
        builder.Services.AddScoped<ISaleService, SaleService>();
        builder.Services.AddScoped<ILoanNoteService, LoanNoteService>();
        builder.Services.AddScoped<IReservationService, ReservationService>();
    }
}