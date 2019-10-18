using Microsoft.VisualStudio.Shell;
using NamespaceFixer.NamespaceBuilder;
using System.ComponentModel;

namespace NamespaceFixer
{
    public class OptionPage : DialogPage, INamespaceAdjusterOptions
    {
        [Category("Options")]
        [DisplayName("Namespace format")]
        [Description(@"
This is the path that will be used when adjusting the namespace of a file. Feel free to change it to match your exacts needs.
Default namespace format:" + NamespaceSections.PROJECT_NAME + NamespaceSections.FILE_TO_PROJECT_PATH + @".
The sections that can be used are:
    • " + NamespaceSections.SOLUTION_NAME + @": just the solution file name.
    • " + NamespaceSections.PROJECT_NAME + @": just the project file name.
    • " + NamespaceSections.PROJECT_ROOT_NAMESPACE + @": the 'Default namespace' specified in the properties of the project.
    • " + NamespaceSections.PROJECT_TO_SOLUTION_PHYSICAL_PATH + @": the path from the project file directory to the solution file directory.
    • " + NamespaceSections.FILE_TO_PROJECT_PATH + @": the physical path from the file adjusting the path of to the project directory.
")]
        public string NamespaceFormat { get; set; } =
            NamespaceSections.PROJECT_NAME +
            NamespaceSections.FILE_TO_PROJECT_PATH;

        [Category("Options")]
        [DisplayName("Extensions of files that will be ignored")]
        [Description("Extensions of files that will be ignored when adjusting namespaces. Please, use ';' to split if more than one.")]
        public string FileExtensionsToIgnore { get; set; } = string.Empty;

        [Category("Options")]
        [DisplayName("Folders that will be ignored")]
        [Description("Folders that will be ignored when adjusting namespaces. Please, use ';' to split if more than one.")]
        public string FoldersToIgnore { get; set; } = string.Empty;
    }
}