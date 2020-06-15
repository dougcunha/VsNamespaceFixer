using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    internal class CsNamespaceBuilderService : LogicNamespaceBuilderService
    {
        protected override string NamespaceStartLimiter => "{" + NewLine;
        protected override string NamespaceEndLimiter => "}";

        public CsNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected override Match FindNamespaceMatch(string fileContent)
            => Regex.Match(fileContent, @"[\r\n|\r|\n]?namespace\s(.+)[\r\n|\r|\n]*{");

        protected override MatchCollection FindUsingMatches(string fileContent)
            => Regex.Matches(fileContent, @"\n?using\s(.+);");

        protected override string BuildNamespaceLine(string desiredNamespace) => "namespace " + desiredNamespace;

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

            ReplaceWithFormat(NamespaceSections.SOLUTION_NAME, solutionName);
            ReplaceWithFormat(NamespaceSections.PROJECT_NAME, projectName);
            ReplaceWithFormat(NamespaceSections.PROJECT_ROOT_NAMESPACE, projectRootNamespace);
            ReplaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_PHYSICAL_PATH, projectToSolutionPhysicalPath);
            ReplaceWithFormat(NamespaceSections.PROJECT_TO_SOLUTION_VIRTUAL_PATH, projectToSolutionVirtualPath);
            ReplaceWithFormat(NamespaceSections.FILE_TO_PROJECT_PATH, fileToProjectPath);

            return newNamespace;
        }
    }
}
