using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace NamespaceFixer.Core
{
    internal class VsItemInfo
    {
        private readonly IVsHierarchy _vsHierarchy;
        private string _name;
        private Project _msBuildProject;

        public VsItemInfo(IVsHierarchy pVsHierarchy)
            => _vsHierarchy = pVsHierarchy;

        public IVsHierarchy GetVsHierarchy()
            => _vsHierarchy;

        public IVsProject GetVsProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // ReSharper disable once SuspiciousTypeConversion.Global
            return _vsHierarchy as IVsProject;
        }

        public string GetProjectFullPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string fullPath = null;
            IVsProject project = GetVsProject();

            project?.GetMkDocument(VSConstants.VSITEMID_ROOT, out fullPath);

            return fullPath;
        }

        public string GetSolutionFullPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return GetMsBuildProjectValue(MsBuildEvaluationHelper.SOLUTION_PATH_PROPERTY);
        }

        public string GetName()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return _name ??= GetName(_vsHierarchy, "no-name");
        }

        private string GetMsBuildProjectValue(string key)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string rslt = null;
            Project msBuildProject = GetMsBuildProject();

            if (msBuildProject != null)
                rslt = msBuildProject.GetPropertyValue(key);

            return rslt;
        }

        private Project GetMsBuildProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_msBuildProject != null)
                return _msBuildProject;

            var fullPath = GetProjectFullPath();

            if (fullPath != null)
                _msBuildProject = MsBuildEvaluationHelper.GetProject(fullPath);

            return _msBuildProject;
        }

        private static string GetName(IVsHierarchy vsHierarchy, string @default)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            object value = GetProperty(vsHierarchy, __VSHPROPID.VSHPROPID_Name, Convert.ToUInt32(VSConstants.VSITEMID.Root));

            return value?.ToString() ?? @default;
        }

        private static object GetProperty(IVsHierarchy vsHierarchy, __VSHPROPID key, uint vsItemId)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            bool success = PackageHelper.Success(vsHierarchy.GetProperty(vsItemId, Convert.ToInt32(key), out var value));

            return success ? value : null;
        }
    }
}
