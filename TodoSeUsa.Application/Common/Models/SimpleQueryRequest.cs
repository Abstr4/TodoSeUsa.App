namespace TodoSeUsa.Application.Common.Models;

public class SimpleQueryRequest
{
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}