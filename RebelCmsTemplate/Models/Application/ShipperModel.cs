namespace RebelCmsTemplate.Models.Application;

public class ShipperModel
{
    public int ShipperKey { get; init; }
    public int TenantKey { get; init; }
    public string? ShipperName { get; init; }
    public string? ShipperPhone { get; init; }
    public int IsDelete { get; init; }
}