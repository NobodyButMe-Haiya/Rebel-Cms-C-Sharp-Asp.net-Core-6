namespace RebelCmsTemplate.Models.Application;
public partial class InvoiceModel
{
	public int InvoiceKey { get; init; } 
	public int TenantKey { get; init; } 
	public int CustomerKey { get; init; } 
	public int ShipperKey { get; init; } 
	public int EmployeeKey { get; init; } 
	public DateOnly? InvoiceOrderDate { get; init; } 
	public DateOnly? InvoiceRequiredDate { get; init; } 
	public DateOnly? InvoiceShippedDate { get; init; } 
	public decimal InvoiceFreight { get; init; } 
	public string? InvoiceShipName { get; init; } 
	public string? InvoiceShipAddress { get; init; } 
	public string? InvoiceShipCity { get; init; } 
	public string? InvoiceShipRegion { get; init; } 
	public string? InvoiceShipPostalCode { get; init; } 
	public string? InvoiceShipCountry { get; init; } 
	public int IsDelete { get; init; } 
}
public partial class InvoiceModel
{
	public string? TenantName { get; init; } 
	public string? CustomerName { get; init; } 
	public string? ShipperName { get; init; } 
	public string? EmployeeLastName { get; init; } 
}
public partial class InvoiceModel
{

	// @todo  we cannot auto declare this thing as models  .. data is enough
	public List<InvoiceDetailModel>? Data { get; set; }
}
