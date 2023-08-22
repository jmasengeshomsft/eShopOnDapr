namespace Microsoft.eShopOnDapr.BlazorClient.Catalog;

public record CatalogItem(
    int Id,
    string Name,
    decimal Price,
    string PictureFileName)
{
    public string GetFormattedPrice() => Price.ToString("0.00");

    public string GetPictureUrl(Settings settings)
    {
        if(settings.ApiGatewayType == "Envoy")
        {
            return $"{settings.ApiGatewayUrlExternal}/c/pics/{PictureFileName}";     
        }
        else
        {
            return $"{settings.ApiGatewayUrlExternal}/c/api/v1/catalog/items/{Id}/pic";
        }     
    } 
}
