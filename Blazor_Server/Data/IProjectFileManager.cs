using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    public interface IProjectFileManager
    {
        static string Delimiter { get; }

        /// <summary>
        /// Saves in coming stream.
        /// </summary>
        /// <param name="stream">Stream to save.</param>
        /// <param name="fileName">File Extention of attachment.</param>
        /// <returns>File name the file was stored under.</returns>
        Task<string> SaveAsync(Stream stream, string fileName);

        Task<bool> DeleteAsync(string fileLocation, IPrincipal principal);

        Task<Stream> ReadAsync(string fileLocation);
    }
}
