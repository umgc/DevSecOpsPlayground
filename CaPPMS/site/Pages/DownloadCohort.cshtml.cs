using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CaPPMS.Data;
using CaPPMS.Model;

namespace CaPPMS.Pages
{
    /// <summary>
    /// Downloads all project attachments for a given cohort (class) as a single zip archive.
    /// Route: /download/cohort/{classId}
    /// </summary>
    public class DownloadCohortModel : PageModel
    {
        private readonly ProjectManagerService projectManager;

        public DownloadCohortModel(ProjectManagerService projectManager)
        {
            this.projectManager = projectManager;
        }

        public async Task<IActionResult> OnGetAsync(long classId)
        {
            var cohortProjects = projectManager.ProjectIdeas
                .Values
                .Where(p => p.ClassId == classId)
                .ToList();

            if (!cohortProjects.Any())
            {
                return NotFound();
            }

            using var zipMemoryStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var project in cohortProjects)
                {
                    foreach (var attachment in project.Attachments)
                    {
                        var fileLocation = attachment.Location;
                        if (string.IsNullOrEmpty(fileLocation))
                        {
                            continue;
                        }

                        using var fileStream = await projectManager.FileManager.ReadAsync(fileLocation);
                        if (fileStream == null)
                        {
                            continue;
                        }

                        // Use ProjectID (GUID) and File_ID (GUID) in the zip path so
                        // entries never collide, even when multiple teams in the same
                        // cohort have same-titled projects or same-named attachments.
                        var entryName =
                            $"cohort-{classId}/project-{project.ProjectID}/{attachment.File_ID}_{SanitizeFileName(attachment.Name)}";
                        var zipEntry = zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);

                        using var zipEntryStream = zipEntry.Open();
                        await fileStream.CopyToAsync(zipEntryStream);
                    }
                }
            }

            zipMemoryStream.Position = 0;

            var fileName = $"cohort-{classId}-projects.zip";
            return File(zipMemoryStream.ToArray(), "application/zip", fileName);
        }

        /// <summary>
        /// Strips path separators and other unsafe characters from an attachment
        /// name so it can safely be used as a zip entry segment.
        /// </summary>
        private static string SanitizeFileName(string name)
        {
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}
