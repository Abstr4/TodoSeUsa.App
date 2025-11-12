using System.Linq.Expressions;
using TodoSeUsa.Application.Common.Enums;

namespace TodoSeUsa.Application.Common.Services;
public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> BuildPredicate<T>(QueryRequest request)
    {
        try
        {
            if (request.Filters == null || request.Filters.Count == 0)
                return x => true;

            var param = Expression.Parameter(typeof(T), "x");
            Expression? finalExpr = null;

            foreach (var f in request.Filters)
            {
                var member = Expression.Property(param, f.Property);
                var memberType = member.Type;
                var underlyingType = Nullable.GetUnderlyingType(memberType) ?? memberType;
                var filterValue = f.FilterValue;

                Expression filterExpr;

                if (f.FilterOperator == FilterOperator.IsNull || f.FilterOperator == FilterOperator.IsNotNull)
                {
                    if (!memberType.IsClass && Nullable.GetUnderlyingType(memberType) == null)
                    {
                        filterExpr = Expression.Constant(f.FilterOperator == FilterOperator.IsNotNull);
                    }
                    else
                    {
                        filterExpr = f.FilterOperator == FilterOperator.IsNull
                            ? Expression.Equal(member, Expression.Constant(null, memberType))
                            : Expression.NotEqual(member, Expression.Constant(null, memberType));
                    }
                }
                else
                {
                    if (filterValue != null)
                    {
                        if (underlyingType.IsEnum && filterValue is int intValue)
                            filterValue = Enum.ToObject(underlyingType, intValue);

                        else if (filterValue.GetType() != underlyingType)
                            filterValue = Convert.ChangeType(filterValue, underlyingType);
                    }

                    var constant = Expression.Constant(filterValue, memberType);
                    filterExpr = BuildFilterExpression(member, constant, f.FilterOperator);
                }

                finalExpr = finalExpr == null
                    ? filterExpr
                    : request.LogicalFilterOperator == LogicalFilterOperator.And
                        ? Expression.AndAlso(finalExpr, filterExpr)
                        : Expression.OrElse(finalExpr, filterExpr);
            }

            return Expression.Lambda<Func<T, bool>>(finalExpr!, param);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Exception raised while trying to build the predicate: {ex}");
        }
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
            FilterOperator.DoesNotContain => Expression.Not(Expression.Call(member, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant)),
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
