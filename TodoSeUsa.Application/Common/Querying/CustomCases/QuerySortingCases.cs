using TodoSeUsa.Application.Features.Boxes.DTOs;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Consignors.DTOs;

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
                nameof(ConsignmentDto.ConsignorFullName),
                (q, desc) => desc
                    ? q.OrderByDescending(c => c.Consignor.Person.FirstName)
                           .ThenByDescending(c => c.Consignor.Person.LastName)
                    : q.OrderBy(c => c.Consignor.Person.FirstName)
                           .ThenBy(c => c.Consignor.Person.LastName)
            }
            };

    public static readonly Dictionary<string, Func<IQueryable<Product>, bool, IQueryable<Product>>> ProductSorts =
        new()
        {
            {
                nameof(ProductDto.ConsignorInfo),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Consignment.Consignor.Person.FirstName)
                           .ThenByDescending(p => p.Consignment.Consignor.Person.LastName)
                    : q.OrderBy(p => p.Consignment.Consignor.Person.FirstName)
                           .ThenBy(p => p.Consignment.Consignor.Person.LastName)
            }
        };

    public static readonly Dictionary<string, Func<IQueryable<Consignor>, bool, IQueryable<Consignor>>> ConsignorSorts =
        new()
        {
            {
                nameof(ConsignorDto.TotalProducts),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Consignments.Sum(cg => cg.Products.Count))
                    : q.OrderBy(p => p.Consignments.Sum(cg => cg.Products.Count))
            },
            {
                nameof(ConsignorDto.TotalConsignments),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Consignments.Count)
                    : q.OrderBy(p => p.Consignments.Count)
            },
            {
                nameof(ConsignorDto.FullName),
                (q, desc) => desc
                    ? q.OrderByDescending(p => p.Person.FirstName)
                           .ThenByDescending(p => p.Person.LastName)
                    : q.OrderBy(p => p.Person.FirstName)
                           .ThenBy(p => p.Person.LastName)
            },
            {
                nameof(ConsignorDto.ContactInfo),
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