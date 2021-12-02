namespace RebelCmsTemplate.Models.Application
{
    public class InvoiceFollowUpModel
    {
        public int InvoiceFollowUpKey { get; init; }
        public int TenantKey { get; init; }
        public int InvoiceKey { get; init; }
        public int FollowUpKey { get; init; }
        public int FollowUpAttachmentKey { get; set; }
        public string? InvoiceFollowUpDescription { get; init; }
    }
}

