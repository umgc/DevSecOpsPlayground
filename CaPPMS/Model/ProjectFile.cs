using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace CaPPMS.Model
{
    public class ProjectFile : IProjectFile
    {
        public Guid File_ID { get; set; } = Guid.NewGuid();

        public string Location { get; set; } = string.Empty;

        public string Name
        {
            get
            {
                if (BrowserFile != null)
                {
                    return BrowserFile.Name;
                }

                return Location.Split(ProjectFileManager.Delimiter, StringSplitOptions.RemoveEmptyEntries).Last();
            }
        }
        
        [JsonIgnore]
        public IBrowserFile BrowserFile { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
