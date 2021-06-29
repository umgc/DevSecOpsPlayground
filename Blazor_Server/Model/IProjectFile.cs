using Microsoft.AspNetCore.Components.Forms;

namespace Blazor_Server.Model
{
    public interface IProjectFile
    {
        /// <summary>
        /// Files received from the browser.
        /// </summary>
        IBrowserFile BrowserFile { get; set; }

        /// <summary>
        /// Location stored on the backend.
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// Bring the file name forward for easy enumeration.
        /// </summary>
        string Name => BrowserFile.Name;
    }
}
