namespace TodoSeUsa.BlazorServer.UI.Models;

public class Option<T>
{
    public T? Value { get; set; }
    public string Text { get; set; } = string.Empty;
}