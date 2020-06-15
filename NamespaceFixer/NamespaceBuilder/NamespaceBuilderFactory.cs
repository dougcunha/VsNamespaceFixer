using NamespaceFixer.Core;
using System.IO;

namespace NamespaceFixer.NamespaceBuilder
{
    internal static class NamespaceBuilderFactory
    {
        internal static INamespaceBuilder CreateNamespaceBuilderService(string extension, INamespaceAdjusterOptions options, string filePath)
        {
            string projectName = ProjectHelper.GetProjectExtensionName(extension);
            string fileExtension = Path.GetExtension(filePath);

            if (projectName == Statics.CS_PROJECT_FILE_EXTENSION)
                switch (fileExtension)
                {
                    case Statics.CS_FILE_EXTENSION:
                        return new CsNamespaceBuilderService(options);

                    case Statics.XAML_FILE_EXTENSION:
                        return new XamlNamespaceBuilderService(options);
                }
            else if (projectName == Statics.VB_PROJECT_FILE_EXTENSION)
                return new VbNamespaceBuilderService(options);

            return new DummyNamespaceBuilderService();
        }
    }
}