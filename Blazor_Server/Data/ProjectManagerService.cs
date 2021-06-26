using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    public class ProjectManagerService : IIdeaManager
    {
        public event EventHandler ProjectIdeasChanged;

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
                var ideas = JsonConvert.DeserializeObject<Dictionary<string, ProjectInformation>>(File.ReadAllText(projectsDbFile.FullName));
            }

            ProjectFileManager = new LocalProjectFilesManager();

            ProjectIdeasChanged += ProjectManagerService_ProjectIdeasChanged;
        }

        public ConcurrentDictionary<string, ProjectInformation> ProjectIdeas { get; } = new ();

        public IProjectFileManager ProjectFileManager { get; private set; }

        public IEnumerable<string> GetIdeaTitles()
        {
            foreach (var key in this.ProjectIdeas.Keys)
            {
                yield return key;
            }
        }

        public async Task<bool> AddAsync(ProjectInformation idea)
        {
            foreach(var file in idea.Attachements)
            {
                file.Location = await ProjectFileManager.SaveAsync(file.BrowserFile.OpenReadStream(ProjectFile.MaxFileSize), file.Name);
            }

            if (!ProjectIdeas.TryAdd(idea.ProjectTitle, idea))
            {
                return false;
            }

            ProjectIdeasChanged?.Invoke(ProjectIdeas, EventArgs.Empty);
            return true;
        }

        public async Task<bool> RemoveAsync(ProjectInformation idea, IPrincipal user)
        {
            if (this.ProjectIdeas.TryGetValue(idea.ProjectTitle, out idea))
            {
                bool didDelete = await Task.Run(async () =>
                {
                    foreach(var file in idea.Attachements)
                    {
                        if(!await this.ProjectFileManager.DeleteAsync(file.Location, user))
                        {
                            return false;
                        }
                    }

                    return true;
                });

                if (didDelete)
                {
                    didDelete = this.ProjectIdeas.Remove(idea.ProjectTitle, out ProjectInformation _);
                    this.ProjectIdeasChanged?.Invoke(this.ProjectIdeas, EventArgs.Empty);
                }
                else
                {
                    throw new InvalidOperationException($"Could not finish the operation. Either the user did not have permissions or the file no longer exists");
                }

                return didDelete;
            }

            return false;
        }

        private void ProjectManagerService_ProjectIdeasChanged(object sender, EventArgs e)
        {
            var tempFile = new FileInfo(Path.Combine(localProjectDb + ".temp"));

            // Let's build a gate to control flow. It might be a bit extra but it should be fun.
            Task.Run(async () =>
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
                   Task.Run(async () =>
                   {
                       File.WriteAllText(tempFile.FullName, JsonConvert.SerializeObject(ProjectIdeas, Formatting.Indented));

                       while (!File.Exists(tempFile.FullName))
                       {
                           await Task.Delay(100);
                       }
                   }).ContinueWith((context) =>
                   {
                       tempFile.MoveTo(localProjectDb, true);
                   });
               }
           });
        }

        public Task<bool> UpdateAsync(ProjectInformation idea)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(ProjectInformation idea)
        {
            throw new NotImplementedException();
        }

        public Task<string> ExportAsync(ProjectInformation idea)
        {
            throw new NotImplementedException();
        }
    }
}
