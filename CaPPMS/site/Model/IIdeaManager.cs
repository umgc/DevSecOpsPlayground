using System.Security.Principal;
using System.Threading.Tasks;

namespace CaPPMS.Model
{
    public interface IIdeaManager
    {
        /// <summary>
        /// The maximum number of files a user can upload.
        /// </summary>
        int MaxNumberOfFiles { get; }

        /// <summary>
        /// The maximum size in MB a file can be.
        /// </summary>
        long MaxMBSizePerFile { get; }

        /// <summary>
        /// Update project on system.
        /// </summary>
        /// <param name="projectInformation"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(ProjectInformation projectInformation);

        /// <summary>
        /// Delete project from system.
        /// </summary>
        /// <param name="projectInformation"></param>
        /// <returns>Error</returns>
        Task<string> DeleteAsync(ProjectInformation projectInformation, IPrincipal user);

        /// <summary>
        /// Export project from system.
        /// </summary>
        /// <param name="idea"></param>
        /// <returns></returns>
        Task<string> ExportAsync(ProjectInformation idea);
    }
}
