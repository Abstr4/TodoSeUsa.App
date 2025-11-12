using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.BlazorServer.Helpers;

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
            return "";

        return season switch
        {
            Season.Spring => "Primavera",
            Season.Summer => "Verano",
            Season.Autumn => "Otoño",
            Season.Winter => "Invierno",
            _ => season.ToString()!
        };
    }

    public static string TranslateStatus(ProductStatus status) => status switch
    {
        ProductStatus.Available => "Disponible",
        ProductStatus.Sold => "Vendido",
        ProductStatus.Loaned => "Prestado",
        ProductStatus.Reserved => "Reservado",
        ProductStatus.Discontinued => "Descontinuado",
        _ => status.ToString()
    };
}
