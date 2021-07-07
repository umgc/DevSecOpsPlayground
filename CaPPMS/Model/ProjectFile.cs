using Microsoft.AspNetCore.Components.Forms;
using System;

namespace CaPPMS.Model
{
    public class ProjectFile : IProjectFile
    {
        public Guid File_ID { get; set; } = Guid.NewGuid();

        public const long MaxFileSize = 1024 * 1024 * 2;

        public string Location { get; set; } = string.Empty;

        public string Name => BrowserFile.Name;
        
        public IBrowserFile BrowserFile { get; set; }
    }
}
