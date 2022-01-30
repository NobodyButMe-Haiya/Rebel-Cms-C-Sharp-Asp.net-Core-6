namespace RebelCmsTemplate.Models.Administrator;

public partial class UserModel
{
    public uint UserKey { get; init; }
    public uint TenantKey { get; init; }
    public uint RoleKey { get; init; }
    public string? UserName { get; init; }
    public string? UserPassword { get; set; }
    public string? UserAddress { get; init; }
    public string? UserEmail { get; init; }
    public string? UserPhone { get; init; }
}

public partial class UserModel
{
    public string? RoleName { get; set; }
    public string? TenantName { get; set; }
}