using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NamespaceFixer.InnerPathFinder;
using NamespaceFixer.SolutionSelection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NamespaceFixer.Core
{
    using System.Linq;

    /// <summary>
    /// Information provider on Visual Studio.
    /// Based on the visual studio shell.
    /// </summary>
    internal class VsServiceInfo
    {
        private IVsSolutionBuildManager _vsSolutionBuildManager;
        private IVsMonitorSelection _vsMonitorSelection;

        private VsItemInfo _startupProject;
        private FileInfo _solutionFile;

        internal NamespaceAdjuster NamespaceAdjuster { get; }
        internal ISolutionSelectionService SolutionSelectionService { get; }
        internal IInnerPathFinder InnerPathFinder { get; }

        public VsServiceInfo(NamespaceAdjuster namespaceAdjuster)
        {
            NamespaceAdjuster = namespaceAdjuster;
            SolutionSelectionService = new SolutionSelectionService();
            InnerPathFinder = new InnerPathFinderService();
        }

        /// <summary>
        /// Returns the build manager of the solution.
        /// </summary>
        /// <returns></returns>
        public IVsSolutionBuildManager GetVsSolutionBuildManager()
            => _vsSolutionBuildManager ??= PackageHelper.GetService<IVsSolutionBuildManager>(typeof(SVsSolutionBuildManager));

        /// <summary>
        /// Returns the selection monitor.
        /// </summary>
        /// <returns></returns>
        public IVsMonitorSelection GetVsMonitorSelection()
            => _vsMonitorSelection ??= PackageHelper.GetService<IVsMonitorSelection>(typeof(SVsShellMonitorSelection));

        /// <summary>
        /// Returns the startup project (based on the build manager).
        /// </summary>
        /// <returns></returns>
        public VsItemInfo GetStartupProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_startupProject != null)
                return _startupProject;

            IVsSolutionBuildManager solutionBuildManager = GetVsSolutionBuildManager();

            if (solutionBuildManager == null)
                return _startupProject;

            bool success = PackageHelper.Success(solutionBuildManager.get_StartupProject(out IVsHierarchy value));

            if (success && value != null)
                _startupProject = new VsItemInfo(value);

            return _startupProject;
        }

        /// <summary>
        /// Returns the solution file (based on the startup project).
        /// </summary>
        /// <returns></returns>
        public FileInfo GetSolutionFileInfo()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_solutionFile != null)
                return _solutionFile;

            VsItemInfo startupProject = GetStartupProject();

            string solutionFullPath = startupProject?.GetSolutionFullPath();

            if (solutionFullPath != null)
                _solutionFile = new FileInfo(solutionFullPath);

            return _solutionFile;
        }

        public IList<IVsHierarchy> GetSelectedProjects()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsMonitorSelection vsMonitorSelection = GetVsMonitorSelection();

            if (vsMonitorSelection == null)
                return null;

            bool success = PackageHelper.Success(vsMonitorSelection.GetCurrentSelection(out IntPtr hierarchyPtr, out uint itemId, out IVsMultiItemSelect multiSelect, out IntPtr containerPtr));

            if (IntPtr.Zero != containerPtr)
                Marshal.Release(containerPtr);

            if (!success)
                return null;

            var rslt = new List<IVsHierarchy>();

            if (itemId == (uint)VSConstants.VSITEMID.Selection && multiSelect != null)
            {
                success = PackageHelper.Success(multiSelect.GetSelectionInfo(out uint itemCount, out _));

                if (!success)
                    return rslt;

                VSITEMSELECTION[] items = new VSITEMSELECTION[itemCount];

                success = PackageHelper.Success(multiSelect.GetSelectedItems(0, itemCount, items));

                if (!success)
                    return rslt;

                foreach (var item in items.Where(item => item.pHier != null && !rslt.Contains(item.pHier)))
                    rslt.Add(item.pHier);
            }
            else if (hierarchyPtr != IntPtr.Zero)
            {
                object uniqueObjectForIUnknown = Marshal.GetUniqueObjectForIUnknown(hierarchyPtr);

                if (!(uniqueObjectForIUnknown is IVsHierarchy))
                    return rslt;

                IVsHierarchy hierarchy = (IVsHierarchy)uniqueObjectForIUnknown;
                rslt.Add(hierarchy);
            }

            return rslt;
        }
    }
}
