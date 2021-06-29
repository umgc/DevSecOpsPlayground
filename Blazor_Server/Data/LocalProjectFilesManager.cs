using Blazor_Server.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    public class LocalProjectFilesManager : IProjectFileManager
    {
        public static string Delimiter { get; } = "----";

        public LocalProjectFilesManager()
        {
            if (!FileDirInfo.Exists)
            {
                FileDirInfo.Create();

                // Reassign it to force object update based on new directory.
                FileDirInfo = new DirectoryInfo(FileDirInfo.FullName);
            }
        }

        public DirectoryInfo FileDirInfo = new (Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "files"));

        /// <summary>
        /// For Test method only. Checks to ensure delete is from debug folder.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string fileLocation)
        {
            return await DeleteAsync(fileLocation, true);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(string fileLocation, IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
            {
                Debug.WriteLine($"User is not authenticated: {principal.Identity.Name}");
                return false;
            }

            var filePath = Path.Combine(this.FileDirInfo.FullName, fileLocation);

            filePath = Path.GetFullPath(filePath);            

            if (!filePath.StartsWith(this.FileDirInfo.FullName))
            {
                // Maybe log this in
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                return false;
            }

            if (!principal.IsInRole("owner"))
            {
                // Maybe log this in
                Debug.WriteLine($"User:{principal.Identity.Name} does not have permission to delete the file");
                return false;
            }


            return await DeleteAsync(fileLocation, false);
        }

        /// <inheritdoc/>
        public async Task<Stream> ReadAsync(string fileLocation)
        {
            var file = new FileInfo(Path.Combine(FileDirInfo.FullName, fileLocation));

            if (!file.Exists)
            {
                return null;
            }

            return await Task.FromResult(file.OpenRead());
        }

        /// <inheritdoc/>
        public async Task<string> SaveAsync(Stream stream, string fileName)
        {
            fileName = Guid.NewGuid().ToString() + Delimiter + fileName;
            var filePath = Path.Combine(FileDirInfo.FullName, (string)fileName);

            filePath = Path.GetFullPath(filePath);
            
            if (!filePath.StartsWith(FileDirInfo.FullName))
            {
                // Maybe log this in
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                return null;
            }

            if (File.Exists(filePath))
            {
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                return null;
            }

            using(var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            return fileName;
        }

        private async Task<bool> DeleteAsync(string fileLocation, bool fromTest)
        {
            Regex testLocation = new Regex(@"[\\/][Bb]in[\\/][Dd]ebug[\\/]");

            if (fromTest && !testLocation.IsMatch(fileLocation))
            {
                return false;
            }

            return await Task.Run(() =>
            {
                if (File.Exists(fileLocation))
                {
                    File.Delete(fileLocation);
                    return true;
                }

                return false;
            });
        }
    }
}
