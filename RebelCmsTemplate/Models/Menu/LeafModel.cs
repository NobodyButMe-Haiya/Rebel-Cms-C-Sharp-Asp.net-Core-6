namespace RebelCmsTemplate.Models.Menu
{
    public class LeafModel
    {
        public int LeafKey { get; init; }
        public int LeafSeq { get; init; }
        public int FolderKey { get; init; }
        public string? LeafFilename { get; init; }
        public string? LeafName { get; init; }
        public string? LeafIcon { get; init; }
        public int IsDelete { get; init; }
    }
}
