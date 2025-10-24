using System.Linq.Dynamic.Core;

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

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, string? filter)
    {
        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(filter);

        return query;
    }
}
