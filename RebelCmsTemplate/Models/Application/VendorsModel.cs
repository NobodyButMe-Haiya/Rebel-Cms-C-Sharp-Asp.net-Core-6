namespace RebelCmsTemplate.Models.Application;

public class VendorsModel
{
    public int VendorKey { get; init; }
    public int VendorTypeKey { get; init; }
    public string? VendorName { get; init; }
    public string? VendorTitle { get; init; }
    public string? VendorAddress { get; init; }
    public string? VendorCity { get; init; }
    public string? VendorCounty { get; init; }
    public string? VendorRegion { get; init; }
    public string? VendorState { get; init; }
    public string? VendorCountry { get; init; }
    public string? VendorPostCode { get; init; }
    public string? VendorPhone { get; init; }
}