using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace NamespaceFixer.SolutionSelection
{
    internal class SolutionSelectionService : ISolutionSelectionService
    {
        public string[] GetSelectedItemsPaths()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedItems = GetSelectedItems();

            return selectedItems == null
                ? Array.Empty<string>()
                : (from UIHierarchyItem selItem in selectedItems
                    select (ProjectItem)selItem.Object into prjItem
                    select prjItem.Properties.Item("FullPath")
                        .Value.ToString()).ToArray();
        }

        private Array GetSelectedItems()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // ReSharper disable once InconsistentNaming
            var _applicationObject = GetDTE2();

            var uih = _applicationObject.ToolWindows.SolutionExplorer;

            return (Array)uih.SelectedItems;
        }

        private EnvDTE80.DTE2 GetDTE2()
            => Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
    }
}