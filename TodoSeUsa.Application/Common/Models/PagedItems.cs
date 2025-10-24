namespace TodoSeUsa.Application.Common.Models;

public record PagedItems<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int Count { get; init; }
}