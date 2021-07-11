using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CaPPMS.Model
{
    public abstract class ProjectFileManager : IProjectFileManager
    {
        public static string Delimiter { get; } = "----";

        public abstract string DownloadPath { get; }

        public abstract DirectoryInfo FileDirInfo { get; }

        public abstract Task<string> DeleteAsync(string fileLocation, IPrincipal principal);

        public abstract Task<Stream> ReadAsync(string fileLocation);

        public abstract Task<string> SaveAsync(Stream stream, string fileId, string fileName);
    }
}
