using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    [Authorize]
    public class LocalProjectFilesManager : IProjectFileManager
    {
        private DirectoryInfo baseLocation = new (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredData"));

        public async Task<bool> Delete(string fileLocation, IPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
            {
                Debug.WriteLine($"User is not authenticated: {principal.Identity.Name}");
                return false;
            }

            var filePath = Path.Combine(baseLocation.FullName, fileLocation);

            filePath = Path.GetFullPath(filePath);            

            if (!filePath.StartsWith(baseLocation.FullName))
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

            return await Task.Run(() =>
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            });
        }

        public Task<Stream> Read(string fileLocation)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save(Stream stream, string fileLocation)
        {
            var filePath = Path.Combine(baseLocation.FullName, fileLocation);

            filePath = Path.GetFullPath(filePath);
            
            if (!filePath.StartsWith(baseLocation.FullName))
            {
                // Maybe log this in
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                return false;
            }

            if (File.Exists(filePath))
            {
                Debug.WriteLine($"Path not within the specified storage location. Attempted path: {filePath}");
                return false;
            }

            using(var fs = new FileStream(fileLocation, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            return true;
        }
    }
}
