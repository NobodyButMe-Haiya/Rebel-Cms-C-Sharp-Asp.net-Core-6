namespace RebelCmsTemplate.Models.Menu;

public class FolderModel
{
    public int FolderKey { get; init; }
    public string? FolderName { get; init; }
    public string? FolderFilename { get; init; }
    public string? FolderIcon { get; init; }
    public int FolderSeq { get; init; }
    public int IsDelete { get; init; }
}