using System.Threading.Tasks;

namespace Blazor_Server.Model
{
    public interface IIdeaManager
    {
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
        /// <returns></returns>
        Task<bool> DeleteAsync(ProjectInformation projectInformation);

        /// <summary>
        /// Export project from system.
        /// </summary>
        /// <param name="idea"></param>
        /// <returns></returns>
        Task<string> ExportAsync(ProjectInformation idea);
    }
}
