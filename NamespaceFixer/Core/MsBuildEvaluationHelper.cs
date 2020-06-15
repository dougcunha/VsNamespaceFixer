using Microsoft.Build.Evaluation;
using System.Linq;

namespace NamespaceFixer.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MsBuildEvaluationHelper
    {
        /// <summary>
        /// Absolute path to the solution file.
        /// Example : J:\Repos\Tr4ncer\VsNamespaceFixer\NamespaceFixer.sln
        /// </summary>
        public const string SOLUTION_PATH_PROPERTY = "SolutionPath";

        private static ProjectCollection _allProjects;

        /// <summary>
        /// Cleaning cached variables.
        /// </summary>
        public static void ClearCache()
        {
            if (_allProjects == null)
                return;

            _allProjects.Dispose();
            _allProjects = null;
        }

        /// <summary>
        /// Returns the known project.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static Project GetProject(string fullPath)
            => GetAllProjects().GetLoadedProjects(fullPath).FirstOrDefault();

        /// <summary>
        /// Returns all known projects.
        /// </summary>
        /// <returns></returns>
        private static ProjectCollection GetAllProjects()
            => _allProjects ??= ProjectCollection.GlobalProjectCollection;
    }
}
