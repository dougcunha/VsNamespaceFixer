using System;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class VbNamespaceBuilderService : NamespaceBuilderService
    {
        protected override string NamespaceStartLimiter => string.Empty;
        protected override string NamespaceEndLimiter => "End Namespace";

        public VbNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent)
        {
            return Regex.Match(fileContent, @"[\r\n|\r|\n]?Namespace\s(.+)[\r\n|\r|\n]");
        }

        protected override MatchCollection FindUsingMatches(string fileContent)
        {
            return Regex.Matches(fileContent, @"[\r\n|\r|\n]?Imports\s(.+)[\r\n|\r|\n]");
        }

        protected override string BuildNamespaceLine(string desiredNamespace)
        {
            return "Namespace " + desiredNamespace;
        }

        internal override string BuildNamespaceAccordingToOptions(
            string solutionName,
            string projectName,
            string projectRootNamespace,
            string projectToSolutionPhysicalPath,
            string projectToSolutionVirtualPath,
            string fileToProjectPath)
        {
            var newNamespace = GetOptions().NamespaceFormat;

            void replaceWithFormat(string namespaceSection, string sectionValue)
            {
                newNamespace = newNamespace.Replace(namespaceSection, "/" + sectionValue);
            }

            replaceWithFormat(NamespaceSections.SOLUTION_NAME, String.Empty);
            replaceWithFormat(NamespaceSections.PROJECT_NAME, String.Empty);
            replaceWithFormat(NamespaceSections.PROJECT_ROOT_NAMESPACE, String.Empty);
            replaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_PHYSICAL_PATH, String.Empty);
            replaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_VIRTUAL_PATH, String.Empty);
            replaceWithFormat(NamespaceSections.FILE_TO_PROJECT_PATH, fileToProjectPath);

            return newNamespace;
        }
    }
}