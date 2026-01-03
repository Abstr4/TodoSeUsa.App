namespace TodoSeUsa.Application.Common.Models;

public sealed record UploadedFile
{
    public long Size { get; init; }
    public DateTimeOffset LastModified { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public Stream Content { get; init; } = Stream.Null;
}
