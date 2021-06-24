using Microsoft.AspNetCore.Components.Forms;

namespace Blazor_Server.Data
{
    public interface IProjectFile
    {
        IBrowserFile BrowserFile { get; set; }

        string Location { get; set; }

        string Name => BrowserFile.Name;
    }
}
