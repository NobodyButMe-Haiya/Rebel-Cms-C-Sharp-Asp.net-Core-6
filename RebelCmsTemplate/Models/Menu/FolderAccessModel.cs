namespace RebelCmsTemplate.Models.Menu;

public partial class FolderAccessModel
{
    public int FolderAccessKey { get; init; }
    public int FolderKey { get; init; }
    public int RoleKey { get; init; }
    public int FolderAccessValue { get; init; }
}

// this is foreign value so don't want to spoil 
public partial class FolderAccessModel
{
    public string? RoleName { get; init; }
    public string? FolderName { get; init; }
}