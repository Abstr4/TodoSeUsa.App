using TodoSeUsa.Application.Common.Querying.Enums;

namespace TodoSeUsa.Application.Common.Models;

public class SortDescriptor
{
    public string Property { get; set; } = string.Empty;
    public SortOrder SortOrder { get; set; }
}