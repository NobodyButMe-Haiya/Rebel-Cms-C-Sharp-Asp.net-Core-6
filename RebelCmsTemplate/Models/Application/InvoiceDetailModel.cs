namespace RebelCmsTemplate.Models.Application;

public class InvoiceDetailModel
{
    public uint InvoiceDetailKey { get; init; }
    public uint InvoiceKey { get; init; }
    public uint ProductKey { get; init; }
    public decimal InvoiceDetailUnitPrice { get; init; }
    public int InvoiceDetailQuantity { get; init; }
    public double InvoiceDetailDiscount { get; init; }
    public int IsDelete { get; init; }
}