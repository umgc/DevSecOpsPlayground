using CaPPMS.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaPPMS.Data
{
    public class LocalProjectFilesManager : ProjectFileManager
    {
        public LocalProjectFilesManager()
        {
            if (!FileDirInfo.Exists)
            {
                FileDirInfo.Create();

                // Reassign it to force object update based on new directory.
                FileDirInfo = new DirectoryInfo(FileDirInfo.FullName);
            }
        }

        public override DirectoryInfo FileDirInfo { get; } = new(Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "files"));

        public override string DownloadPath => "/download/";

        /// <summary>
        /// For Test method only. Checks to ensure delete is from debug folder.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public Task<string> DeleteAsync(string fileLocation)
        {
            return Task.FromResult(Delete(fileLocation, true));
        }

        /// <inheritdoc/>
        public override Task<string> DeleteAsync(string fileLocation, IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
            {
                return Task.FromResult("User is not authenticated");
            }

            //if (!principal.IsInRole("Contributor"))
            //{
            //    return $"User:{principal.Identity.Name} does not have permission to delete {fileLocation.Split(Delimiter).Last()}.";
            //}

            var filePath = Path.Combine(this.FileDirInfo.FullName, fileLocation);

            filePath = Path.GetFullPath(filePath);

            if (!filePath.StartsWith(this.FileDirInfo.FullName))
            {
                // Maybe log this in
                return Task.FromResult($"Path not within the specified storage location. User:{principal?.Identity?.Name}. Attempted path:{filePath}");
            }

            return Task.FromResult(Delete(filePath, false));
        }

        /// <inheritdoc/>
        public override async Task<Stream> ReadAsync(string fileLocation)
        {
            var file = new FileInfo(Path.Combine(FileDirInfo.FullName, fileLocation));

            if (!file.Exists)
            {
                return null;
            }

            return await Task.FromResult(file.OpenRead());
        }

        /// <inheritdoc/>
        public override async Task<string> SaveAsync(Stream stream, string fileId, string fileName)
        {
            fileName = fileId + Delimiter + fileName;
            var filePath = Path.Combine(FileDirInfo.FullName, fileName);

            filePath = Path.GetFullPath(filePath);
            
            if (!filePath.StartsWith(FileDirInfo.FullName))
            {
                // Maybe log this in
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                return null;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using(var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                Task writeTask = stream.CopyToAsync(fs);

                await writeTask;

                if (writeTask.IsFaulted)
                {
                    throw new IOException("Failed to save the incoming stream to the file system.", writeTask.Exception);
                }
            }

            int wait = -1;

            while(!File.Exists(filePath) && wait++ < 3)
            {
                await Task.Delay(300);
            }

            return fileName;
        }

        /// <summary>
        /// This method is used to enable testable results. The test will not specifically be able to delete like an authenticated user so we will enable with this helper method.
        /// </summary>
        /// <param name="fileLocation">Location of the File</param>
        /// <param name="fromTest">If from test or not.</param>
        /// <returns></returns>
        private string Delete(string fileLocation, bool fromTest)
        {
            Regex testLocation = new Regex(@"[\\/][Bb]in[\\/][Dd]ebug[\\/]");

            if (fromTest && !testLocation.IsMatch(fileLocation))
            {
                return "File access denied";
            }

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
                return null;
            }

           return "File does not exist.";
        }
    }
}
