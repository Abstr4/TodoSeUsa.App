using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? orderBy,
        Func<IQueryable<T>, string, IQueryable<T>?>? customSorting = null,
        string defaultOrderBy = "Id")
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query.OrderBy(defaultOrderBy);

        try
        {
            var customQuery = customSorting?.Invoke(query, orderBy);
            if (customQuery is not null)
                return customQuery;

            return query.OrderBy(orderBy);
        }
        catch
        {
            return query.OrderBy(defaultOrderBy);
        }
    }

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, IEnumerable<SortDescriptor>? sorts)
    {
        var sort = sorts?.FirstOrDefault();
        if (sort == null || string.IsNullOrWhiteSpace(sort.Property))
            return query;

        var property = sort.Property;
        var isDescending = sort.SortOrder == SortOrder.Descending;

        return isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, property))
            : query.OrderBy(e => EF.Property<object>(e, property));
    }

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string? filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var config = new ParsingConfig
            {
                ResolveTypesBySimpleName = true
            };

            // Register your enum types directly in the constructor
            config.CustomTypeProvider = new DefaultDynamicLinqCustomTypeProvider(
                config,
                [ typeof(ProductStatus), typeof(ProductQuality), typeof(Season), typeof(PaymentStatus), typeof(PaymentMethod), typeof(LoanedProductStatus), typeof(LoanNoteStatus), typeof(ReservationStatus) ]
            );

            query = query.Where(config, filter);
        }

        return query;
    }

}
