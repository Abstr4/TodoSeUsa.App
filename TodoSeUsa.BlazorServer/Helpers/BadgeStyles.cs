using Radzen;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.BlazorServer.Helpers;

public static class BadgeStyles
{
    public static BadgeStyle GetBadgeStyleFromProductStatus(ProductStatus status)
    {
        return status switch
        {
            ProductStatus.Available => BadgeStyle.Success,
            ProductStatus.Sold => BadgeStyle.Info,
            ProductStatus.Reserved => BadgeStyle.Primary,
            ProductStatus.Discontinued => BadgeStyle.Danger,
            ProductStatus.Loaned => BadgeStyle.Warning,
            _ => BadgeStyle.Base
        };
    }

    public static BadgeStyle GetBadgeStyleFromStringProductStatus(string status)
    {
        return status switch
        {
            "Available" => BadgeStyle.Success,
            "Sold" => BadgeStyle.Info,
            "Reserved" => BadgeStyle.Primary,
            "Discontinued" => BadgeStyle.Danger,
            "Loaned" => BadgeStyle.Warning,
            _ => BadgeStyle.Base
        };
    }
}