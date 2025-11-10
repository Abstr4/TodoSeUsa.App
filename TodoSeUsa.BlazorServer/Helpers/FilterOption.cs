namespace TodoSeUsa.BlazorServer.Helpers;

public class FilterOption<T>
{
    public T? Value { get; set; }
    public string Text { get; set; } = string.Empty;
}