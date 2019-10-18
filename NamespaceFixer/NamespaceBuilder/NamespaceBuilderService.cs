﻿using System;
using System.IO;
using System.IO.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace NamespaceFixer.NamespaceBuilder
{
    public abstract class NamespaceBuilderService : INamespaceBuilder
    {
        private readonly INamespaceAdjusterOptions _options;

        protected abstract string NamespaceStartLimiter { get; }

        protected abstract string NamespaceEndLimiter { get; }

        protected NamespaceBuilderService(INamespaceAdjusterOptions options)
        {
            _options = options;
        }

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
            {
                result = Regex.Replace(result, $@"(^|\.){folder}(\.|$)", ".", RegexOptions.IgnoreCase);
            }

            return ToValidFormat(result);
        }

        protected abstract Match FindNamespaceMatch(string fileContent);

        protected abstract MatchCollection FindUsingMatches(string fileContent);

        protected abstract string BuildNamespaceLine(string desiredNamespace);

        public bool UpdateFile(ref string fileContent, string desiredNamespace)
        {
            if (string.IsNullOrEmpty(desiredNamespace)) return false;

            SetNewLine(fileContent);

            var namespaceMatch = FindNamespaceMatch(fileContent);

            return namespaceMatch.Success ?
                UpdateNamespace(ref fileContent, desiredNamespace, namespaceMatch) :
                CreateNamespace(ref fileContent, desiredNamespace);
        }

        private void SetNewLine(string fileContent)
        {
            var isCrlf = fileContent.IndexOf("\r\n") > -1;

            NewLine = isCrlf ? "\r\n" : "\n";
        }

        internal INamespaceAdjusterOptions GetOptions()
        {
            return _options;
        }

        internal abstract string BuildNamespaceAccordingToOptions(
         string solutionName,
         string projectName,
         string projectRootNamespace,
         string projectToSolutionPhysicalPath,
         string projectToSolutionVirtualPath,
         string fileToProjectPath);

        private string GetFileToProjectPath(FileInfo projectFile, string filePath)
        {
            return Directory.GetParent(filePath).FullName.Substring(projectFile.Directory.FullName.Length);
        }

        private string GetProjectToSolutionPhysicalPath(FileInfo solutionFile, FileInfo projectFile)
        {
            string solutionDirectoryFullName = solutionFile.Directory.FullName;
            string projectDirectoryFullName = projectFile.Directory.FullName;

            if (!projectDirectoryFullName.StartsWith(solutionDirectoryFullName))
                return string.Empty;

            var projectAndSolutionFilesAreSameDirectory = projectDirectoryFullName.Equals(solutionDirectoryFullName);

            return projectAndSolutionFilesAreSameDirectory
                ? string.Empty
                : projectDirectoryFullName.Substring(solutionDirectoryFullName.Length + 1);
        }

        private string ToValidFormat(string name)
        {
            return name
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace("\\", "/")
                .Replace('/', '.')
                .Replace("..", ".")
                .Trim('.');
        }

        private string GetRootNamespaceFromProject(FileInfo projectFile)
        {
            using (var reader = BuildXmlProjectFileReader(projectFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "RootNamespace")
                    {
                        reader.Read();

                        return reader.NodeType == XmlNodeType.Text ? reader.Value : null;
                    }
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

        private bool UpdateNamespace(ref string fileContent, string desiredNamespace, Match namespaceMatch)
        {
            var fileRequiresUpdate = false;

            var namespaceGroup = namespaceMatch.Groups.OfType<Group>().FirstOrDefault(g => !(g is Match));

            if (namespaceGroup == null) return false;

            var currentNamespace = namespaceGroup.Value.Trim();

            if (currentNamespace != desiredNamespace)
            {
                fileRequiresUpdate = true;
                fileContent = fileContent.Substring(0, namespaceGroup.Index) + desiredNamespace + fileContent.Substring(namespaceGroup.Index + namespaceGroup.Value.Trim().Length);
            }

            return fileRequiresUpdate;
        }

        private bool CreateNamespace(ref string fileContent, string desiredNamespace)
        {
            var usingMatches = FindUsingMatches(fileContent);
            var lastUsing = usingMatches.OfType<Match>().LastOrDefault();

            string usingSectionContent = string.Empty;
            if (lastUsing != null)
            {
                var indexAfterUsing = lastUsing.Index + lastUsing.Length;
                usingSectionContent = fileContent.Substring(0, indexAfterUsing).Trim();

                fileContent = fileContent.Substring(indexAfterUsing);
            }

            fileContent =
                (string.IsNullOrEmpty(usingSectionContent) ? string.Empty : usingSectionContent + NewLine + NewLine) +
                BuildNamespaceLine(desiredNamespace) + NewLine +
                NamespaceStartLimiter +
                fileContent.Trim() +
                NewLine + NamespaceEndLimiter;

            return true;
        }
    }
}