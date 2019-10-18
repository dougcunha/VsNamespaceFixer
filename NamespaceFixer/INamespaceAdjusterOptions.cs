namespace NamespaceFixer
{
    public interface INamespaceAdjusterOptions
    {
        string NamespaceFormat { get; }

        string FileExtensionsToIgnore { get; }

        string FoldersToIgnore { get; set; }
    }
}