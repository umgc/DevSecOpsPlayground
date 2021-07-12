using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CaPPMS.Model
{
    public interface IProjectFileManager
    {
        DirectoryInfo FileDirInfo { get; }
        string DownloadPath { get; }
        /// <summary>
        /// Saves in coming stream.
        /// </summary>
        /// <param name="stream">Stream to save.</param>
        /// <param name="fileName">File Extention of attachment.</param>
        /// <returns>File name the file was stored under.</returns>
        Task<string> SaveAsync(Stream stream, string fileId, string fileName);

        /// <summary>
        /// Deletes specified file from storage.
        /// </summary>
        /// <param name="fileLocation">Location of the stored file.</param>
        /// <param name="principal">User making the request.</param>
        /// <returns>Error</returns>
        Task<string> DeleteAsync(string fileLocation, IPrincipal principal);

        /// <summary>
        /// Reads the file stream.
        /// </summary>
        /// <param name="fileLocation">The location of the stored file.</param>
        /// <returns>The data as <see cref="Stream"/></returns>
        Task<Stream> ReadAsync(string fileLocation);
    }
}
