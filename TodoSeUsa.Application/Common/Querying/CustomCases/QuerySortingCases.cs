using TodoSeUsa.Application.Features.Boxes.DTOs;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Providers.DTOs;

namespace TodoSeUsa.Application.Common.Querying.CustomCases;

public static class QuerySortingCases
{
    public static readonly Dictionary<string, Func<IQueryable<Box>, bool, IQueryable<Box>>> BoxCustomSorts =
    new()
    {
        {
            nameof(BoxDto.TotalProducts),
            (q, desc) => desc
                ? q.OrderByDescending(b => b.Products.Count)
                : q.OrderBy(b => b.Products.Count)
        }
    };

    public static readonly Dictionary<string, Func<IQueryable<Consignment>, bool, IQueryable<Consignment>>> ConsignmentSorts =
            new()
            {
            {
                nameof(ConsignmentDto.TotalProducts),
                (q, desc) => desc
                    ? q.OrderByDescending(c => c.Products.Count)
                    : q.OrderBy(c => c.Products.Count)
            },
            {
                nameof(ConsignmentDto.ProviderFullName),
                (q, desc) => desc
                    ? q.OrderByDescending(c => c.Provider.Person.FirstName)
                           .ThenByDescending(c => c.Provider.Person.LastName)
                    : q.OrderBy(c => c.Provider.Person.FirstName)
                           .ThenBy(c => c.Provider.Person.LastName)
            }
            };

    public static readonly Dictionary<string, Func<IQueryable<Product>, bool, IQueryable<Product>>> ProductSorts =
        new()
        {
            {
                nameof(ProductDto.ProviderInfo),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Consignment.Provider.Person.FirstName)
                           .ThenByDescending(p => p.Consignment.Provider.Person.LastName)
                    : q.OrderBy(p => p.Consignment.Provider.Person.FirstName)
                           .ThenBy(p => p.Consignment.Provider.Person.LastName)
            }
        };

    public static readonly Dictionary<string, Func<IQueryable<Provider>, bool, IQueryable<Provider>>> ProviderSorts =
        new()
        {
            {
                nameof(ProviderDto.TotalProducts),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Consignments.Sum(cg => cg.Products.Count))
                    : q.OrderBy(p => p.Consignments.Sum(cg => cg.Products.Count))
            },
            {
                nameof(ProviderDto.TotalConsignments),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Consignments.Count)
                    : q.OrderBy(p => p.Consignments.Count)
            },
            {
                nameof(ProviderDto.FullName),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Person.FirstName)
                           .ThenByDescending(p => p.Person.LastName)
                    : q.OrderBy(p => p.Person.FirstName)
                           .ThenBy(p => p.Person.LastName)
            },
            {
                nameof(ProviderDto.ContactInfo),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Person.EmailAddress)
                           .ThenByDescending(p => p.Person.PhoneNumber)
                                .ThenByDescending(p => p.Person.Address)
                    : q.OrderBy(p => p.Person.FirstName)
                           .ThenBy(p => p.Person.PhoneNumber)
                                .ThenBy(p => p.Person.Address)
            },
        };
}