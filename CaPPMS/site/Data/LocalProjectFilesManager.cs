using CaPPMS.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaPPMS.Data
{
    public class LocalProjectFilesManager : ProjectFileManager
    {
        private const string CompressedExtension = ".gz";

        public LocalProjectFilesManager()
        {
            if (!FileDirInfo.Exists)
            {
                FileDirInfo.Create();

                // Reassign it to force object update based on new directory.
                FileDirInfo = new DirectoryInfo(FileDirInfo.FullName);
            }
        }

        /// <summary>
        /// One-time migration: compresses every legacy uncompressed file in the
        /// storage directory into a .gz variant, then removes the original.
        /// Naturally idempotent — once every file is stored as .gz this is a
        /// no-op and the method (plus its call site in Program.cs) can be
        /// deleted after PPE and Live have both been rolled out with this
        /// change.
        /// </summary>
        /// <remarks>
        /// TODO: [rwilson127] Remove this method and the Program.cs call site
        /// once all files on PPE and Live have been migrated to .gz.
        /// </remarks>
        public async Task MigrateExistingFilesAsync()
        {
            if (!FileDirInfo.Exists)
            {
                return;
            }

            var rawFiles = FileDirInfo.GetFiles("*", SearchOption.TopDirectoryOnly)
                .Where(f => !f.Name.EndsWith(CompressedExtension, StringComparison.OrdinalIgnoreCase));

            foreach (var rawFile in rawFiles)
            {
                var compressedPath = rawFile.FullName + CompressedExtension;

                // Skip if a compressed variant already exists.
                if (File.Exists(compressedPath) && IsWithinFileDir(compressedPath))
                {
                    if (rawFile.Exists)
                    {
                        rawFile.Delete();
                    }
                    continue;
                }

                // Read the entire file into memory first, then write the compressed
                // variant. This avoids file-share locking issues on Windows where the
                // raw file may still be held by the caller that created it.
                var rawBytes = await File.ReadAllBytesAsync(rawFile.FullName);

                using var outputStream = File.Create(compressedPath);
                using var gzip = new GZipStream(outputStream, CompressionMode.Compress);
                await gzip.WriteAsync(rawBytes, 0, rawBytes.Length);

                rawFile.Delete();
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
            if (!principal.Identity?.IsAuthenticated ?? false)
            {
                return Task.FromResult("User is not authenticated");
            }

            //if (!principal.IsInRole("Contributor"))
            //{
            //    return $"User:{principal.Identity.Name} does not have permission to delete {fileLocation.Split(Delimiter).Last()}.";
            //}

            var filePath = Path.GetFullPath(Path.Combine(this.FileDirInfo.FullName, fileLocation));

            if (!IsWithinFileDir(filePath))
            {
                // Maybe log this in
                return Task.FromResult($"Path not within the specified storage location. User:{principal?.Identity?.Name}. Attempted path:{filePath}");
            }

            // Clean up both compressed (.gz) and legacy (raw) variants so a
            // migration between formats leaves no orphans behind.
            bool deleted = false;
            var compressedPath = filePath + CompressedExtension;
            if (File.Exists(compressedPath) && IsWithinFileDir(compressedPath))
            {
                File.Delete(compressedPath);
                deleted = true;
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                deleted = true;
            }

            return Task.FromResult(deleted ? null : "File does not exist.");
        }

        /// <inheritdoc/>
        public override async Task<Stream?> ReadAsync(string fileLocation)
        {
            var fullPath = Path.GetFullPath(Path.Combine(FileDirInfo.FullName, fileLocation));

            if (!IsWithinFileDir(fullPath))
            {
                return null;
            }

            // Prefer the compressed (.gz) variant and hydrate (decompress) on serve.
            var compressedPath = fullPath + CompressedExtension;
            if (File.Exists(compressedPath) && IsWithinFileDir(compressedPath))
            {
                using var fileStream = File.OpenRead(compressedPath);
                var memoryStream = new MemoryStream();
                using (var gzip = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    await gzip.CopyToAsync(memoryStream);
                }
                memoryStream.Position = 0;
                return memoryStream;
            }

            // Fall back to legacy uncompressed files written before compression was introduced.
            var file = new FileInfo(fullPath);

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
            var filePath = Path.GetFullPath(Path.Combine(FileDirInfo.FullName, fileName));

            if (!IsWithinFileDir(filePath))
            {
                // Maybe log this in
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                throw new UnauthorizedAccessException($"Path not within the specified storage location. Attempted path: {filePath}");
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var compressedPath = filePath + CompressedExtension;
            if (File.Exists(compressedPath))
            {
                File.Delete(compressedPath);
            }

            // Compress every file on store; the download path hydrates (decompresses) on serve.
            using(var fs = new FileStream(compressedPath, FileMode.Create, FileAccess.Write))
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                using(var gzip = new GZipStream(fs, CompressionMode.Compress))
                {
                    await stream.CopyToAsync(gzip);
                }
            }

            return fileName;
        }

        /// <summary>
        /// This method is used to enable testable results. The test will not specifically be able to delete like an authenticated user so we will enable with this helper method.
        /// When called from tests, the file must live under a build output folder (bin/Debug or bin/Release).
        /// </summary>
        /// <param name="fileLocation">Location of the File</param>
        /// <param name="fromTest">If from test or not.</param>
        /// <returns></returns>
        private string Delete(string fileLocation, bool fromTest)
        {
            Regex testLocation = new Regex(@"[\\/][Bb]in[\\/](Debug|Release)[\\/]");

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

        /// <summary>
        /// Ensures a resolved path stays inside the storage directory so callers cannot escape it via ".." segments.
        /// </summary>
        private bool IsWithinFileDir(string fullPath)
        {
            string dir = FileDirInfo.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            return fullPath.StartsWith(dir, StringComparison.Ordinal);
        }
    }
}
