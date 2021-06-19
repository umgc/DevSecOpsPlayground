using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blazor_Server.Data
{
    interface IProjectFileManager
    {
        Task<bool> Save(Stream stream, string fileLocation);

        Task<bool> Delete(string fileLocation, IPrincipal principal);

        Task<Stream> Read(string fileLocation);
    }
}
