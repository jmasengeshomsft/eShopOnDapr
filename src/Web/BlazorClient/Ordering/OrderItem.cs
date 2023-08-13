namespace Microsoft.eShopOnDapr.BlazorClient.Ordering;

public record OrderItem(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Units,
    string PictureFileName)
{
    public decimal Total => UnitPrice * Units;

    public string GetFormattedUnitPrice() => UnitPrice.ToString("0.00");

    public string GetFormattedTotal() => Total.ToString("0.00");

    public string GetPictureUrl(Settings settings)
    {
        if(settings.ApiGatewayType == "APIM")
        {
            return $"{settings.ApiGatewayUrlExternal}/c/api/v1/catalog/items/{ProductId}/pic";      
        }
        else
        {
            return $"{settings.ApiGatewayUrlExternal}/c/pics/{PictureFileName}";
        } 
    } 
}
