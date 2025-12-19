using TodoSeUsa.BlazorServer.UI.Models;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.BlazorServer.Enums;

public static class EnumOptionLists
{
    public static readonly IEnumerable<Option<ProductQuality?>> qualities =
        [.. Enum.GetValues<ProductQuality>()
            .Cast<ProductQuality>()
            .Select(q => new Option<ProductQuality?>
            {
                Value = q,
                Text = EnumTranslate.TranslateQuality(q)
            })
        ];

    public static readonly IEnumerable<Option<Body?>> bodies =
        [.. Enum.GetValues<Body>()
            .Cast<Body>()
            .Select(b => new Option<Body?>
            {
                Value = b,
                Text = EnumTranslate.TranslateBody(b)
            })
        ];

    public static readonly IEnumerable<Option<Season?>> seasons =
        [.. Enum.GetValues<Season>()
            .Cast<Season>()
            .Select(s => new Option<Season?>
            {
                Value = s,
                Text = EnumTranslate.TranslateSeason(s)
            })
        ];

    public static readonly IEnumerable<Option<PaymentMethod?>> paymentMethods =
    [.. Enum.GetValues<PaymentMethod>()
            .Cast<PaymentMethod>()
            .Select(s => new Option<PaymentMethod?>
            {
                Value = s,
                Text = EnumTranslate.TranslatePaymentMethod(s)
            })
    ];
}