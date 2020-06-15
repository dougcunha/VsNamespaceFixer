using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NamespaceFixer.NamespaceBuilder
{
    /// <summary>
    /// This NamespaceBuilder is meant for code, like .cs or .vb files.
    /// </summary>
    internal abstract class LogicNamespaceBuilderService : NamespaceBuilderService
    {
        protected LogicNamespaceBuilderService(INamespaceAdjusterOptions options) : base(options)
        {
        }

        protected abstract string NamespaceStartLimiter { get; }

        protected abstract string NamespaceEndLimiter { get; }

        public override bool UpdateFile(ref string fileContent, string desiredNamespace)
        {
            if (string.IsNullOrEmpty(desiredNamespace)) return false;

            SetNewLine(fileContent);

            var namespaceMatch = FindNamespaceMatch(fileContent);

            return namespaceMatch.Success ?
                UpdateNamespace(ref fileContent, desiredNamespace, namespaceMatch) :
                CreateNamespace(ref fileContent, desiredNamespace);
        }

        protected abstract MatchCollection FindUsingMatches(string fileContent);

        protected abstract string BuildNamespaceLine(string desiredNamespace);

        private void SetNewLine(string fileContent)
        {
            var isCrlf = fileContent.IndexOf("\r\n", StringComparison.Ordinal) > -1;

            NewLine = isCrlf
                ? "\r\n"
                : "\n";
        }

        private bool UpdateNamespace(ref string fileContent, string desiredNamespace, Match namespaceMatch)
        {
            var namespaceGroup = namespaceMatch.Groups.OfType<Group>().FirstOrDefault(g => !(g is Match));

            if (namespaceGroup == null)
                return false;

            var currentNamespace = namespaceGroup.Value.Trim();

            if (currentNamespace == desiredNamespace)
                return false;

            fileContent = fileContent.Substring(0, namespaceGroup.Index) + desiredNamespace + fileContent.Substring(namespaceGroup.Index + namespaceGroup.Value.Trim().Length);

            return true;
        }

        private bool CreateNamespace(ref string fileContent, string desiredNamespace)
        {
            var usingMatches = FindUsingMatches(fileContent);
            var lastUsing = usingMatches.OfType<Match>().LastOrDefault();

            var usingSectionContent = string.Empty;

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