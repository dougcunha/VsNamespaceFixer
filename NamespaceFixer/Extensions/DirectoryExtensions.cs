namespace NamespaceFixer.Extensions
{
    using System.IO;

    public static class DirectoryExtensions
    {
        public static bool IsSamePath(this DirectoryInfo current, DirectoryInfo other)
            => current.FullName == other.FullName;

        public static bool IsRoot(this DirectoryInfo current)
            => current.FullName == Directory.GetDirectoryRoot(current.FullName);
    }
}