using System.Linq.Expressions;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Providers.DTOs;

namespace TodoSeUsa.Application.Common.Helpers;
public static class QueryFilteringCases
{
    public static readonly Dictionary<string, Func<string, Expression<Func<Product, bool>>>> ProductFilters =
    new()
    {
        {
            nameof(ProductDto.ProviderInfo),
            val => c =>
                EF.Functions.Like(c.Consignment.Provider.Person.FirstName, $"%{val}%") ||
                EF.Functions.Like(c.Consignment.Provider.Person.LastName, $"%{val}%") ||
                EF.Functions.Like(c.Consignment.ProviderId.ToString(), $"%{val}%")
        }
    };

    public static readonly Dictionary<string, Func<string, Expression<Func<Consignment, bool>>>> ConsignmentFilters =
    new()
    {
        {
            nameof(ConsignmentDto.ProviderFullName),
            val => c =>
                EF.Functions.Like(c.Provider.Person.FirstName, $"%{val}%") ||
                EF.Functions.Like(c.Provider.Person.LastName, $"%{val}%")
        }
    };

    public static readonly Dictionary<string, Func<string, Expression<Func<Provider, bool>>>> ProviderFilters =
    new()
    {
            {
                nameof(ProviderDto.FullName),
                val => c =>
                    EF.Functions.Like(c.Person.FirstName, $"%{val}%") ||
                    EF.Functions.Like(c.Person.LastName, $"%{val}%")
            },
                        {
                nameof(ProviderDto.ContactInfo),
                val => c =>
                    EF.Functions.Like(c.Person.EmailAddress, $"%{val}%") ||
                    EF.Functions.Like(c.Person.PhoneNumber, $"%{val}%") ||
                    EF.Functions.Like(c.Person.Address, $"%{val}%")
            }
    };
}