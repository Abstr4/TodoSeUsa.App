namespace TodoSeUsa.BlazorServer.UI.RadzenAdapters.Files;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

public static class FormFileConverter
{
    public static IFormFile ToFormFile(IBrowserFile browserFile)
    {
        if (browserFile == null)
            throw new ArgumentNullException(nameof(browserFile), "Radzen.FileInfo.Source is null");

        var stream = browserFile.OpenReadStream(long.MaxValue);
        return new FormFile(stream, 0, browserFile.Size, browserFile.Name, browserFile.Name)
        {
            Headers = new HeaderDictionary(),
            ContentType = browserFile.ContentType
        };
    }

    public static IReadOnlyList<IFormFile> ToFormFiles(IEnumerable<IBrowserFile> browserFiles)
    {
        return [.. browserFiles.Select(ToFormFile)];
    }
}
