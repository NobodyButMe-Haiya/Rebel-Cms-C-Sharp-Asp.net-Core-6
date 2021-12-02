namespace RebelCmsTemplate.Models.Administrator
{
    public class ConfigurationModel
    {
        public int ConfigurationKey { get; init; }
        public string? ConfigurationPortal { get; init; }
        public string? ConfigurationPortalLocal { get; init; }
        public string? ConfigurationEmailHost { get; init; }
        public string? ConfigurationEmail { get; init; }
        public string? ConfigurationEmailPassword { get; init; }
        public string? ConfigurationEmailPort { get; init; }
        public string? ConfigurationEmailSecure { get; init; }
        
    }
}
