using System;

namespace NamespaceFixer.Core
{
    internal static class Statics
    {
        /// <summary>
        /// Version of the extension.
        ///
        /// To update in:
        /// - below.
        /// - assemblyinfo.
        /// - source.extension.vsixmanifest
        /// </summary>
        public const string PACKAGE_VERSION = "2.8";

        public const string CS_PROJECT_FILE_EXTENSION = "csproj";
        public const string VB_PROJECT_FILE_EXTENSION = "vbproj";
        public const string CS_FILE_EXTENSION = ".cs";
        public const string XAML_FILE_EXTENSION = ".xaml";
    }

    /// <summary>
    /// Must match guid in .vsct.
    /// </summary>
    internal static class Guids
    {
        public static readonly Guid NamespaceFixerCmdSet = new Guid("{19492BCB-32B3-4EC3-8826-D67CD5526653}");
        public const string NAMESPACE_FIXER_PACKAGE = "3C7C5ABE-82AC-4A37-B077-0FF60E8B1FD3";

        // TODO https://docs.microsoft.com/visualstudio/extensibility/how-to-use-rule-based-ui-context-for-visual-studio-extensions
        //public const string UiContextSupportedFiles = "C694C8AD-1300-4ADC-93D8-EBFA2915354B";
    }

    internal static class Ids
    {
        public const int CMD_ID_ADJUST_NAMESPACE = 0x2001;
    }
}