using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class VbNamespaceBuilderService : LogicNamespaceBuilderService
    {
        protected override string NamespaceStartLimiter => string.Empty;
        protected override string NamespaceEndLimiter => "End Namespace";

        public VbNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent) => Regex.Match(fileContent, @"[\r\n|\r|\n]?Namespace\s(.+)[\r\n|\r|\n]");

        protected override MatchCollection FindUsingMatches(string fileContent) =>
            Regex.Matches(fileContent, @"[\r\n|\r|\n]?Imports\s(.+)[\r\n|\r|\n]");

        protected override string BuildNamespaceLine(string desiredNamespace) =>
            "Namespace " + desiredNamespace;

        protected override string BuildNamespaceAccordingToOptions(
            string solutionName,
            string projectName,
            string projectRootNamespace,
            string projectToSolutionPhysicalPath,
            string projectToSolutionVirtualPath,
            string fileToProjectPath)
        {
            var newNamespace = GetOptions().NamespaceFormat;

            void ReplaceWithFormat(string namespaceSection, string sectionValue)
            {
                newNamespace = newNamespace.Replace(namespaceSection, "/" + sectionValue);
            }

            ReplaceWithFormat(NamespaceSections.SOLUTION_NAME, string.Empty);
            ReplaceWithFormat(NamespaceSections.PROJECT_NAME, string.Empty);
            ReplaceWithFormat(NamespaceSections.PROJECT_ROOT_NAMESPACE, string.Empty);
            ReplaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_PHYSICAL_PATH, string.Empty);
            ReplaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_VIRTUAL_PATH, string.Empty);
            ReplaceWithFormat(NamespaceSections.FILE_TO_PROJECT_PATH, fileToProjectPath);

            return newNamespace;
        }
    }
}
