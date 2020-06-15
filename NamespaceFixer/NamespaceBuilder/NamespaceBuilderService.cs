using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NamespaceFixer.NamespaceBuilder
{
    using NamespaceFixer.Extensions;

    public abstract class NamespaceBuilderService : INamespaceBuilder
    {
        private readonly INamespaceAdjusterOptions _options;

        public NamespaceBuilderService(INamespaceAdjusterOptions options)
            => _options = options;

        protected string NewLine { get; set; } = Environment.NewLine;

        public string GetNamespace(string filePath, FileInfo solutionFile, FileInfo projectFile)
        {
            var solutionName = solutionFile.NameWithoutExtension();
            var projectName = projectFile.NameWithoutExtension();
            var projectRootNamespace = GetRootNamespaceFromProject(projectFile);
            var projectToSolutionPhysicalPath = GetProjectToSolutionPhysicalPath(solutionFile, projectFile);
            var projectToSolutionVirtualPath = string.Empty; // GetProjectToSolutionVirtualPath(solutionFile, projectFile);
            var fileToProjectPath = GetFileToProjectPath(projectFile, filePath);

            string result = BuildNamespaceAccordingToOptions(
                solutionName,
                projectName,
                projectRootNamespace,
                projectToSolutionPhysicalPath,
                projectToSolutionVirtualPath,
                fileToProjectPath);

            result = ToValidFormat(result);

            foreach (var folder in _options.FoldersToIgnore.Split(';'))
                result = Regex.Replace(result, $@"(^|\.){folder}(\.|$)", ".", RegexOptions.IgnoreCase);

            return ToValidFormat(result);
        }

        public abstract bool UpdateFile(ref string fileContent, string desiredNamespace);

        protected abstract Match FindNamespaceMatch(string fileContent);


        protected INamespaceAdjusterOptions GetOptions() => _options;

        protected abstract string BuildNamespaceAccordingToOptions(
            string solutionName,
            string projectName,
            string projectRootNamespace,
            string projectToSolutionPhysicalPath,
            string projectToSolutionVirtualPath,
            string fileToProjectPath);

        private string GetFileToProjectPath(FileInfo projectFile, string filePath) =>
            Directory.GetParent(filePath).FullName.Substring(projectFile.Directory?.FullName.Length ?? 0);

        private string GetProjectToSolutionPhysicalPath(FileInfo solutionFile, FileInfo projectFile)
        {
            var solutionDirectoryFullName = solutionFile.Directory?.FullName;
            var projectDirectoryFullName = projectFile.Directory?.FullName;

            if (!projectDirectoryFullName?.StartsWith(solutionDirectoryFullName ?? string.Empty) == true)
                return string.Empty;

            var projectAndSolutionFilesAreSameDirectory = projectDirectoryFullName?.Equals(solutionDirectoryFullName) == true;

            return projectAndSolutionFilesAreSameDirectory
                ? string.Empty
                : projectDirectoryFullName?.Substring(solutionDirectoryFullName?.Length ?? 0 + 1);
        }

        private static string ToValidFormat(string name) => name
            .Replace(' ', '_')
            .Replace('-', '_')
            .Replace("\\", "/")
            .Replace('/', '.')
            .Replace("..", ".")
            .Trim('.');

        private string GetRootNamespaceFromProject(FileInfo projectFile)
        {
            using (var reader = BuildXmlProjectFileReader(projectFile))
            {
                while (reader.Read())
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "RootNamespace")
                    {
                        reader.Read();

                        return reader.NodeType == XmlNodeType.Text ? reader.Value : null;
                    }
            }

            return Path.GetFileNameWithoutExtension(projectFile.FullName);
        }

        private XmlReader BuildXmlProjectFileReader(FileInfo projectFile)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            return XmlReader.Create(projectFile.FullName, settings);
        }
    }
}
