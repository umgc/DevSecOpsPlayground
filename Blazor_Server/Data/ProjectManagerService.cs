using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    public class ProjectManagerService
    {
        public event EventHandler ProjectIdeasChanged;

        private readonly string localProjectDb = Path.Combine(BaseDirInfo.FullName, "projects.json");

        public static DirectoryInfo BaseDirInfo = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredData"));

        // Limit upload per file to 2Mb.
        public long MaxFileSize { get; } = 1024 * 1024 * 2;

        public ProjectManagerService()
        {
            if (!BaseDirInfo.Exists)
            {
                BaseDirInfo.Create();
            }

            var projectsDbFile = new FileInfo(localProjectDb);

            if (projectsDbFile.Exists)
            {
                var ideas = JsonConvert.DeserializeObject<Dictionary<string, Tuple<IdeaFormModel, HashSet<string>>>>(File.ReadAllText(projectsDbFile.FullName));
            }

            ProjectFileManager = new LocalProjectFilesManager();

            ProjectIdeasChanged += ProjectManagerService_ProjectIdeasChanged;
        }

        public ConcurrentDictionary<string, Tuple<IdeaFormModel, HashSet<string>>> ProjectIdeas { get; } = new ();

        public IProjectFileManager ProjectFileManager { get; private set; }

        public IEnumerable<string> GetIdeaTitles()
        {
            foreach (var key in this.ProjectIdeas.Keys)
            {
                yield return key;
            }
        }

        public async Task<bool> AddAsync(IdeaFormModel idea)
        {
            HashSet<string> files = new HashSet<string>();
            foreach(var file in idea.Attachements)
            {
                files.Add(await ProjectFileManager.SaveAsync(file.OpenReadStream(1024 * 1024 * 2), file.Name));
            }

            if (!ProjectIdeas.TryAdd(idea.ProjectTilte, Tuple.Create(idea, files)))
            {
                return false;    
            }

            ProjectIdeasChanged?.Invoke(ProjectIdeas, EventArgs.Empty);
            return true;
        }

        public async Task RemoveAsync(IdeaFormModel idea, IPrincipal user)
        {
            if (this.ProjectIdeas.TryGetValue(idea.ProjectTilte, out Tuple<IdeaFormModel, HashSet<string>> ideaTuple))
            {
                bool didDelete = await Task.Run(async () =>
                {
                    foreach(var file in ideaTuple.Item2)
                    {
                        if(!await this.ProjectFileManager.DeleteAsync(file, user))
                        {
                            return false;
                        }
                    }

                    return true;
                });

                if (didDelete)
                {
                    _ = this.ProjectIdeas.Remove(idea.ProjectTilte, out Tuple<IdeaFormModel, HashSet<string>> _);
                    this.ProjectIdeasChanged?.Invoke(this.ProjectIdeas, EventArgs.Empty);
                }
                else
                {
                    throw new InvalidOperationException($"Could not finish the operation. Either the user did not have permissions or the file no longer exists");
                }
            }
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
    }
}
