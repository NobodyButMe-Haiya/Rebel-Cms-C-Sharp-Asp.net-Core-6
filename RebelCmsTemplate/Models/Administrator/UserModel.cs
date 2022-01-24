namespace RebelCmsTemplate.Models.Administrator;

public partial class UserModel
{
    public int UserKey { get; init; }
    public int TenantKey { get; init; }
    public int RoleKey { get; init; }
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