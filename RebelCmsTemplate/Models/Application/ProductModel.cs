namespace RebelCmsTemplate.Models.Application;

public partial class ProductModel
{
    public int ProductKey { get; init; }
    public int TenantKey { get; init; }
    public int VendorKey { get; init; }
    public int ProductCategoryKey { get; init; }
    public int ProductTypeKey { get; init; }
    public string? ProductName { get; init; }
    public string? ProductDescription { get; init; }
    public double ProductCostPrice { get; init; }
    public double ProductSellingPrice { get; init; }

    public string? ProductQuantityPerUnit { get; init; }

    public double ProductUnitsInStock { get; init; }

    public double ProductUnitsOnOrder { get; init; }
}

/// <summary>
/// Additional Join information 
/// </summary>
public partial class ProductModel
{
    public string? ProductCategoryName { get; init; }
    public string? ProductTypeName { get; init; }
}