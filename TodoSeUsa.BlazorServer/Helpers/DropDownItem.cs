namespace TodoSeUsa.BlazorServer.Helpers;

public class DropdownItem<T>
{
    public T Value { get; set; } = default!;
    public string Display { get; set; } = string.Empty;
}