namespace Microsoft.eShopOnDapr.BlazorClient;

public class Settings
{
    public string ApiGatewayUrlExternal { get; set; } = null!;
    public string ApiGatewayType { get; set; } = "Envoy";
    public string IdentityUrlExternal { get; set; } = null!;
}
