namespace RebelCmsTemplate.Models.Setting
{
    public partial class ProductTypeModel
    {
    
        public int ProductTypeKey { get; init; }
        public int TenantKey { get; init; }
        public int ProductCategoryKey { get; init; }
        public string? ProductTypeName { get; init; }
    }
    public partial class ProductTypeModel
    {
        public string? ProductCategoryName { get; init; }
        public string? TenantName { get; init; }
    }
}
