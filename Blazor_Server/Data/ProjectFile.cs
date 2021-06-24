using Microsoft.AspNetCore.Components.Forms;

namespace Blazor_Server.Data
{
    public class ProjectFile : IProjectFile
    {
        public const long MaxFileSize = 1024 * 1024 * 2;

        public IBrowserFile BrowserFile { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Name => BrowserFile.Name;
    }
}
