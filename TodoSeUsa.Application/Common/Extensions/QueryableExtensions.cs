using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Application.Common.Services;

namespace TodoSeUsa.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyCustomSorting<T>(
        IQueryable<T> query,
        List<SortDescriptor>? sorts,
        Dictionary<string, Func<IQueryable<T>, bool, IQueryable<T>>>? customSorts = null)
    {
        var sort = sorts?.FirstOrDefault();

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

    public static IQueryable<T> ApplyCustomFiltering<T>(
    IQueryable<T> query,
    List<FilterDescriptor>? filters,
    LogicalFilterOperator logicalFilterOperator,
    Dictionary<string, Func<string, Expression<Func<T, bool>>>>? customFilters = null)
    {
        if (filters == null || filters.Count == 0)
            return query;

        var predicate = PredicateBuilder.BuildPredicate(filters, logicalFilterOperator, customFilters);
        return query.Where(predicate);
    }
}

