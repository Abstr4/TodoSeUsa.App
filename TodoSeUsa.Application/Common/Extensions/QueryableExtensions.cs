using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyCustomSorting<T>(
        IQueryable<T> query,
        SortDescriptor? sort,
        Dictionary<string, Func<IQueryable<T>, bool, IQueryable<T>>>? customSorts = null)
    {
        var isDescending = sort?.SortOrder == SortOrder.Descending;

        if (sort == null || string.IsNullOrWhiteSpace(sort.Property))
        {
            return query.OrderByDescending(p => EF.Property<object>(p!, "CreatedAt"))
                        .ThenByDescending(p => EF.Property<object>(p!, "Id"));
        }

        if (customSorts != null && customSorts.TryGetValue(sort.Property, out var handler))
            return handler(query, isDescending);

        return isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e!, sort.Property))
            : query.OrderBy(e => EF.Property<object>(e!, sort.Property));
    }
}

