namespace RebelCmsTemplate.Models.Application;

public class InvoiceDetailModel
{
    public int InvoiceDetailKey { get; init; }
    public int InvoiceKey { get; init; }
    public int ProductKey { get; init; }
    public decimal InvoiceDetailUnitPrice { get; init; }
    public int InvoiceDetailQuantity { get; init; }
    public double InvoiceDetailDiscount { get; init; }
    public int IsDelete { get; init; }
}