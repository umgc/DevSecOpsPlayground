using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CaPPMS.Data;
using System.IO;

namespace CaPPMS.Pages
{
    public class DownloadModel : PageModel
    {
        ProjectManagerService projectManager;

        public DownloadModel([FromServices] ProjectManagerService projectManager)
        {
            this.projectManager = projectManager;
        }

        public async Task<IActionResult> OnGet()
        {
#if (!DEBUG)
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
#endif
            if (!HttpContext.Request.Path.HasValue)
            {
                return null;
            }

            var fileLocation = HttpContext.Request.Path.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();

            if (string.IsNullOrEmpty(fileLocation))
            {
                return null;
            }

            return File(await GetData(fileLocation), "application/force-download", GetFileName(fileLocation));
        }

        private string GetFileName(string fileLocation)
        {
            if (fileLocation.IndexOf(LocalProjectFilesManager.Delimiter) > 0)
            {
                return fileLocation.Substring(fileLocation.IndexOf(LocalProjectFilesManager.Delimiter) + LocalProjectFilesManager.Delimiter.Length);
            }

            return fileLocation;
        }

        private async Task<byte[]> GetData(string fileLocation)
        {
            using(var ms = new MemoryStream())
            {
                using (var stream = await projectManager.ProjectFileManager.ReadAsync(fileLocation))
                {
                    await stream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
