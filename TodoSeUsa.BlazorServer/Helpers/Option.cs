namespace TodoSeUsa.BlazorServer.Helpers;

public class Option<T>
{
    public T? Value { get; set; }
    public string Text { get; set; } = string.Empty;
}