using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.BlazorServer.Enums;

public static class EnumTranslate
{
    public static string TranslateQuality(ProductQuality quality) => quality switch
    {
        ProductQuality.Poor => "Mala",
        ProductQuality.Fair => "Aceptable",
        ProductQuality.Good => "Buena",
        ProductQuality.LikeNew => "Como nueva",
        ProductQuality.New => "Nueva",
        _ => quality.ToString()
    };

    public static string TranslateBody(Body quality) => quality switch
    {
        Body.Unisex => "Unisex",
        Body.Male => "Masculino",
        Body.Female => "Femenino",
        _ => quality.ToString()
    };

    public static string TranslateSeason(Season? season)
    {
        if (season is null)
            return "-";

        return season switch
        {
            Season.Spring => "Primavera",
            Season.Summer => "Verano",
            Season.Autumn => "Otoño",
            Season.Winter => "Invierno",
            _ => season.ToString()!
        };
    }

    public static string TranslateProductStatus(ProductStatus status) => status switch
    {
        ProductStatus.Available => "Disponible",
        ProductStatus.Sold => "Vendido",
        ProductStatus.Loaned => "Prestado",
        ProductStatus.Reserved => "Reservado",
        ProductStatus.Discontinued => "Descontinuado",
        _ => status.ToString()
    };

    public static string TranslateSaleStatus(SaleStatus status) => status switch
    {
        SaleStatus.Pending => "Pendiente",
        SaleStatus.PartiallyPaid => "Parcialmente pagada",
        SaleStatus.Paid => "Pagada",
        SaleStatus.Cancelled => "Cancelada",
        _ => status.ToString()
    };

    public static string TranslatePaymentMethod(PaymentMethod status) => status switch
    {
        PaymentMethod.Cash => "Efectivo",
        PaymentMethod.DebitCard => "Tarjeta de débito",
        PaymentMethod.BankTransfer => "Transferencia Bancaria",
        PaymentMethod.CreditCard => "Tarjeta de crédito",
        PaymentMethod.Other => "Otro",
        _ => status.ToString()
    };
}