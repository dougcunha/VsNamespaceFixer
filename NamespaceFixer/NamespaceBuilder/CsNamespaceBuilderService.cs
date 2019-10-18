using System;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    public class CsNamespaceBuilderService : NamespaceBuilderService
    {
        protected override string NamespaceStartLimiter => "{" + NewLine;
        protected override string NamespaceEndLimiter => "}";

        public CsNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent)
        {
            return Regex.Match(fileContent, @"[\r\n|\r|\n]?namespace\s(.+)[\r\n|\r|\n]+{");
        }

        protected override MatchCollection FindUsingMatches(string fileContent)
        {
            return Regex.Matches(fileContent, @"\n?using\s(.+);");
        }

        protected override string BuildNamespaceLine(string desiredNamespace)
        {
            return "namespace " + desiredNamespace;
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

            replaceWithFormat(NamespaceSections.SOLUTION_NAME, solutionName);
            replaceWithFormat(NamespaceSections.PROJECT_NAME, projectName);
            replaceWithFormat(NamespaceSections.PROJECT_ROOT_NAMESPACE, projectRootNamespace);
            replaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_PHYSICAL_PATH, projectToSolutionPhysicalPath);
            replaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_VIRTUAL_PATH, projectToSolutionVirtualPath);
            replaceWithFormat(NamespaceSections.FILE_TO_PROJECT_PATH, fileToProjectPath);

            return newNamespace;
        }
    }
}