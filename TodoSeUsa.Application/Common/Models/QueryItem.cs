namespace TodoSeUsa.Application.Common.Models;

public record QueryItem(string Filter, string OrderBy, int Skip, int Take);