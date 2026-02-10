using System.Linq.Expressions;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Consignors.DTOs;

namespace TodoSeUsa.Application.Common.Querying.CustomCases;

public static class QueryFilteringCases
{
    public static readonly Dictionary<string, Func<string, Expression<Func<Product, bool>>>> ProductFilters =
    new()
    {
        {
            nameof(ProductDto.ConsignorInfo),
            val => c =>
                EF.Functions.Like(c.Consignment.Consignor.Person.FirstName, $"%{val}%") ||
                EF.Functions.Like(c.Consignment.Consignor.Person.LastName, $"%{val}%") ||
                EF.Functions.Like(c.Consignment.ConsignorId.ToString(), $"%{val}%")
        }
    };

    public static readonly Dictionary<string, Func<string, Expression<Func<Consignment, bool>>>> ConsignmentFilters =
    new()
    {
        {
            nameof(ConsignmentDto.ConsignorFullName),
            val => c =>
                EF.Functions.Like(c.Consignor.Person.FirstName, $"%{val}%") ||
                EF.Functions.Like(c.Consignor.Person.LastName, $"%{val}%")
        }
    };

    public static readonly Dictionary<string, Func<string, Expression<Func<Consignor, bool>>>> ConsignorFilters =
    new()
    {
        {
            nameof(ConsignorDto.FullName),
            val => c =>
                EF.Functions.Like(c.Person.FirstName, $"%{val}%") ||
                EF.Functions.Like(c.Person.LastName, $"%{val}%")
        },
        {
            nameof(ConsignorDto.ContactInfo),
            val => c =>
                EF.Functions.Like(c.Person.EmailAddress, $"%{val}%") ||
                EF.Functions.Like(c.Person.PhoneNumber, $"%{val}%") ||
                EF.Functions.Like(c.Person.Address, $"%{val}%")
        }
    };
}