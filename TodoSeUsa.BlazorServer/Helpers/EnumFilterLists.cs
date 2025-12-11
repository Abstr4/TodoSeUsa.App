using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.BlazorServer.Helpers;

public static class EnumFilterLists
{
    public static readonly IEnumerable<Option<ProductQuality?>> qualities =
    [
        new Option<ProductQuality?> { Value = null, Text = "Todas" },
        .. Enum.GetValues<ProductQuality>()
            .Cast<ProductQuality>()
            .Select(q => new Option<ProductQuality?>
            {
                Value = q,
                Text = EnumTranslate.TranslateQuality(q)
            }),
    ];

    public static readonly IEnumerable<Option<ProductStatus?>> productStatuses =
    [
        new Option<ProductStatus?> { Value = null, Text = "Todos" },
        .. Enum.GetValues<ProductStatus>()
            .Cast<ProductStatus>()
            .Select(s => new Option<ProductStatus?>
            {
                Value = s,
                Text = EnumTranslate.TranslateProductStatus(s)
            }),
    ];

    public static readonly IEnumerable<Option<SaleStatus?>> saleStatuses =
[
    new Option<SaleStatus?> { Value = null, Text = "Todos" },
        .. Enum.GetValues<SaleStatus>()
            .Cast<SaleStatus>()
            .Select(s => new Option<SaleStatus?>
            {
                Value = s,
                Text = EnumTranslate.TranslateSaleStatus(s)
            }),
    ];

    public static readonly IEnumerable<Option<Season?>> seasons =
    [
        new Option<Season?> { Value = null, Text = "Todas" },
        .. Enum.GetValues<Season>()
            .Cast<Season>()
            .Select(s => new Option<Season?>
            {
                Value = s,
                Text = EnumTranslate.TranslateSeason(s)
            }),
    ];

    public static readonly IEnumerable<Option<Body?>> bodies =
    [
        new Option<Body?> { Value = null, Text = "Todos" },
        .. Enum.GetValues<Body>()
            .Cast<Body>()
            .Select(b => new Option<Body?>
            {
                Value = b,
                Text = EnumTranslate.TranslateBody(b)
            }),
    ];
}