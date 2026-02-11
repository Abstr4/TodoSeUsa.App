using System.Linq.Expressions;
using TodoSeUsa.Application.Common.Querying.Enums;

namespace TodoSeUsa.Application.Common.Services;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> BuildPredicate<T>(
    List<FilterDescriptor>? filters,
    LogicalFilterOperator logicalOperator,
    Dictionary<string, Func<string, Expression<Func<T, bool>>>>? customFilters = null)
    {
        if (filters == null || filters.Count == 0)
            return x => true;

        var param = Expression.Parameter(typeof(T), "x");
        Expression? finalExpr = null;

        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Property) || filter.FilterValue == null)
                continue;

            Expression filterExpr;

            // Use custom filter if available
            if (customFilters != null && customFilters.TryGetValue(filter.Property, out var handler))
            {
                var expr = handler(filter.FilterValue.ToString()!);
                filterExpr = Expression.Invoke(expr, param);
            }
            else
            {
                // Default filter building
                var member = Expression.Property(param, filter.Property);
                var memberType = member.Type;
                var underlyingType = Nullable.GetUnderlyingType(memberType) ?? memberType;
                var filterValue = filter.FilterValue;

                if (underlyingType.IsEnum && filterValue is int intValue)
                    filterValue = Enum.ToObject(underlyingType, intValue);
                else if (filterValue.GetType() != underlyingType)
                    filterValue = Convert.ChangeType(filterValue, underlyingType);

                var constant = Expression.Constant(filterValue, memberType);
                filterExpr = BuildFilterExpression(member, constant, filter.FilterOperator);
            }

            finalExpr = finalExpr == null
                ? filterExpr
                : logicalOperator == LogicalFilterOperator.And
                    ? Expression.AndAlso(finalExpr, filterExpr)
                    : Expression.OrElse(finalExpr, filterExpr);
        }

        return finalExpr == null
            ? x => true
            : Expression.Lambda<Func<T, bool>>(finalExpr, param);
    }

    private static Expression BuildFilterExpression(Expression member, Expression constant, FilterOperator op)
    {
        return op switch
        {
            FilterOperator.Equals => Expression.Equal(member, constant),
            FilterOperator.NotEquals => Expression.NotEqual(member, constant),
            FilterOperator.GreaterThan => Expression.GreaterThan(member, constant),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
            FilterOperator.LessThan => Expression.LessThan(member, constant),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
            FilterOperator.Contains => Expression.Call(member, typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
            FilterOperator.DoesNotContain => Expression.Not(Expression.Call(member, typeof(string).GetMethod("Contains", [typeof(string)])!, constant)),
            FilterOperator.StartsWith => Expression.Call(member, typeof(string).GetMethod("StartsWith", [typeof(string)])!, constant),
            FilterOperator.EndsWith => Expression.Call(member, typeof(string).GetMethod("EndsWith", [typeof(string)])!, constant),
            FilterOperator.In => Expression.Call(
                                Expression.Constant(((IEnumerable<object>)((ConstantExpression)constant).Value!).Cast<object>().ToList()),
                                typeof(List<object>).GetMethod("Contains", [typeof(object)])!,
                                Expression.Convert(member, typeof(object))
                            ),
            FilterOperator.NotIn => Expression.Not(
                                Expression.Call(
                                    Expression.Constant(((IEnumerable<object>)((ConstantExpression)constant).Value!).Cast<object>().ToList()),
                                    typeof(List<object>).GetMethod("Contains", [typeof(object)])!,
                                    Expression.Convert(member, typeof(object))
                                )
                            ),
            FilterOperator.IsNull => Expression.Equal(member, Expression.Constant(null)),
            FilterOperator.IsNotNull => Expression.NotEqual(member, Expression.Constant(null)),
            FilterOperator.IsEmpty => Expression.Equal(member, Expression.Constant(string.Empty)),
            FilterOperator.IsNotEmpty => Expression.NotEqual(member, Expression.Constant(string.Empty)),
            _ => throw new NotSupportedException($"Operator {op} not supported"),
        };
    }
}