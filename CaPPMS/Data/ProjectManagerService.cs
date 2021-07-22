using CaPPMS.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CaPPMS.Data
{
    public class ProjectManagerService : IIdeaManager, ICommentManager
    {
        public static event EventHandler ProjectIdeasChanged;

        private readonly string localProjectDb = Path.Combine(BaseDirInfo.FullName, "projects.json");

        public static DirectoryInfo BaseDirInfo { get; } = new (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredData"));

        public ProjectManagerService()
        {
            if (!BaseDirInfo.Exists)
            {
                BaseDirInfo.Create();
            }

            var projectsDbFile = new FileInfo(localProjectDb);

            if (projectsDbFile.Exists)
            {
                var ideas = JsonConvert.DeserializeObject<Dictionary<Guid, ProjectInformation>>(File.ReadAllText(projectsDbFile.FullName));
                foreach (var idea in ideas)
                {
                    _ = ProjectIdeas.TryAdd(idea.Key, idea.Value);
                }
            }

            FileManager = new LocalProjectFilesManager();

            ProjectIdeasChanged += ProjectManagerService_ProjectIdeasChanged;
        }

        public ConcurrentDictionary<Guid, ProjectInformation> ProjectIdeas { get; } = new ();

        public IProjectFileManager FileManager { get; private set; }

        public int MaxNumberOfFiles => Convert.ToInt32(GetConfigurationSetting("MaxNumberOfFiles"));

        public long MaxMBSizePerFile => 1024 * 1024 * Convert.ToInt32(GetConfigurationSetting("MaxMBSizePerFile"));

        public IEnumerable<string> GetIdeaTitles()
        {
            foreach (var project in this.ProjectIdeas.Values)
            {
                yield return project.ProjectTitle;
            }
        }

        public async Task<bool> AddAsync(ProjectInformation idea)
        {
            foreach(var file in idea.Attachments)
            {
                file.Location = await FileManager.SaveAsync(file.BrowserFile.OpenReadStream(MaxMBSizePerFile), file.File_ID.ToString(), file.Name);
            }

            if (!ProjectIdeas.TryAdd(idea.ProjectID, idea))
            {
                return false;
            }

            ProjectIdeasChanged?.Invoke(ProjectIdeas.Values, EventArgs.Empty);
            return true;
        }

        public async Task<string> RemoveAsync(ProjectInformation idea, IPrincipal user)
        {
            string error = string.Empty;

            if (this.ProjectIdeas.TryGetValue(idea.ProjectID, out idea))
            {
                foreach(var file in idea.Attachments)
                {
                    string fileError = await this.FileManager.DeleteAsync(file.Location, user);

                    if (!string.IsNullOrEmpty(fileError) && !fileError.StartsWith("File does not exist."))
                    {
                        error += fileError + "<br/>";
                    }
                }

                // Delete any exported files
                foreach(var file in BaseDirInfo.GetFiles($"{idea.ProjectID}*", SearchOption.AllDirectories))
                {
                    file.Delete();
                }

                if (string.IsNullOrEmpty(error))
                {
                    _ = this.ProjectIdeas.Remove(idea.ProjectID, out ProjectInformation _);
                    ProjectIdeasChanged?.Invoke(this.ProjectIdeas.Values, EventArgs.Empty);
                }
            }

            return error;
        }

        private void ProjectManagerService_ProjectIdeasChanged(object sender, EventArgs e)
        {
            var tempFile = new FileInfo(Path.Combine(localProjectDb + ".temp"));

            // Let's build a gate to control flow. It might be a bit extra but it should be fun.
            _ = Task.Run(async () =>
             {
                 while (File.Exists(tempFile.FullName))
                 {
                   // Use a prime number to reduce the chance of a race condition.
                   await Task.Delay(7);
                 }
             }).ContinueWith((context) =>
             {
               // Update the file backed db.
               lock (ProjectIdeas)
                 {
                   // The task to move some time happens before the os knows that it exists =0
                   _ = Task.Run(async () =>
                       {
                           File.WriteAllText(tempFile.FullName, JsonConvert.SerializeObject(ProjectIdeas, Formatting.Indented));

                           while (!File.Exists(tempFile.FullName))
                           {
                               await Task.Delay(10);
                           }

                           tempFile.MoveTo(localProjectDb,true);
                       });
                 }
             });
        }

        public Task<bool> UpdateAsync(ProjectInformation idea)
        {
            bool completed;
            if (completed = ProjectIdeas.TryUpdate(idea.ProjectID, idea, ProjectIdeas[idea.ProjectID]))
            {
                ProjectIdeasChanged?.Invoke(ProjectIdeas.Values, EventArgs.Empty);
            }

            return Task.FromResult(completed);
        }

        public async Task<string> DeleteAsync(ProjectInformation idea, IPrincipal user)
        {
            return await RemoveAsync(idea, user);
        }
        
        public async Task<string> ExportAsync(ProjectInformation idea)
        {
            int port = 443;
            string hostName = GetConfigurationSetting("Host");

            if (hostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                port = Convert.ToInt32(GetConfigurationSetting("ASPNETCORE_HTTPS_PORT"));
            }

            // Get the fonts used by UMGC.
            // https://www.umgc.edu/documents/upload/umgc-logo-brand-standards.pdf
            var headLineFont = FontFactory.GetFont("Montserrat", 16f);
            var bodyFont = FontFactory.GetFont("Roboto", 12f);

            // Set up the document.
            // TODO: [rwilson127] Page size should be set by user in some setting
            Document pdf = new Document(PageSize.Letter, 25f, 25f, 25f, 35f);
            pdf.AddTitle(idea.ProjectTitle);
            pdf.AddAuthor($"{idea.FirstName} {idea.LastName}");
            pdf.AddCreationDate();
            pdf.AddKeywords("UMGC");
            pdf.AddKeywords("SWEN");
            pdf.AddKeywords("Capstone");
            pdf.AddSubject($"Project Export: {idea.ProjectTitle}");

            // Create the document/
            using(var ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdf, ms);
                var headerTitle = idea.Status.Equals("New") ? $"UMGC Project Proposal:{idea.ProjectTitle}" : $"Approved UMGC Project:{idea.ProjectTitle}";
                writer.PageEvent = new PdfHeaderFooter(headerTitle, $"All Right Reserved {DateTime.Now.Year} SWEN670 Summer 2021", FontFactory.GetFont("Montserrat", 16f, BaseColor.White));
                pdf.Open();

                var headerSpacing = new Paragraph(Environment.NewLine);
                headerSpacing.SpacingAfter = 12f;
                pdf.Add(headerSpacing);

                // Add Exportable Fields.
                // This excludes attachments. Attachments will be added at the end.
                foreach (var exportedField in idea.GetExportableInformation())
                {
                    if (exportedField.Item2 is List<ProjectFile>)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(exportedField.Item2.ToString()))
                    {
                        continue;
                    }

                    var propertyTitleChunk = new Chunk($"{exportedField.Item1}:", headLineFont);
                    propertyTitleChunk.SetUnderline(2f, -2f);
                    var itemTitle = new Paragraph(propertyTitleChunk);
                    itemTitle.SpacingAfter = 1f;
                    itemTitle.Add(Environment.NewLine);
                    var item = new Phrase($"\t\t{exportedField.Item2}", bodyFont);
                    itemTitle.Add(item);

                    pdf.Add(itemTitle);
                }

                var filesTitle = new Paragraph(Environment.NewLine, headLineFont);
                filesTitle.SpacingBefore = 12f;
                filesTitle.SpacingAfter = 9f;
                filesTitle.KeepTogether = true;
                var attachedFilesTitle = new Chunk("Attached Files:", headLineFont);
                attachedFilesTitle.SetUnderline(2f, -2f);
                filesTitle.Add(attachedFilesTitle);
                filesTitle.Add(new Chunk(Environment.NewLine, bodyFont));

                foreach (var attachedFile in idea.Attachments)
                {
                    string host = port == 443 ? hostName : $"{hostName}:{port}";

                    if (Uri.TryCreate($"https://{host}/download/{attachedFile.Location}", UriKind.Absolute, out Uri uri))
                    {
                        var ideaLink = new Anchor(attachedFile.Name, FontFactory.GetFont("Roboto", 12f, BaseColor.Red));
                        ideaLink.Reference = uri.AbsoluteUri;
                        filesTitle.Add(ideaLink);
                        filesTitle.Add(new Phrase(Environment.NewLine));
                    }
                }

                pdf.Add(filesTitle);

                pdf.Close();

                return await FileManager.SaveAsync(ms, idea.ProjectID.ToString(), $"{idea.ProjectTitle}-Contact.pdf");
            }
        }

        private string GetConfigurationSetting(string key)
        {
            foreach (var item in Program.HostProperties)
            {
#pragma warning disable CS0252 // Possible unintended reference comparison; Left side is type
                if (item.Key == typeof(Microsoft.AspNetCore.Hosting.WebHostBuilderContext))
#pragma warning restore CS0252 // Possible unintended reference comparison; Left side is type
                {
                    var context = item.Value as Microsoft.AspNetCore.Hosting.WebHostBuilderContext;

                    return context.Configuration[key];
                }
            }

            return string.Empty;
        }

        public void AddComment(Comment comment)
        {
            if(ProjectIdeas.TryGetValue(comment.ProjectID, out ProjectInformation project))
            {
                project.Comments.Add(comment);
                ProjectIdeasChanged?.Invoke(ProjectIdeas.Values, EventArgs.Empty);
            }
        }

        public IEnumerable<Comment> GetComments(Guid projectID)
        {
            if(ProjectIdeas.TryGetValue(projectID, out ProjectInformation project))
            {
                List<Comment> comments = new List<Comment>();
                foreach(Comment comment in project.Comments.Values)
                {
                    comments.Add(comment);
                }

                return comments.OrderByDescending(c => c.UpdateDateTime);
            }

            return new List<Comment>();
        }
    }
}
