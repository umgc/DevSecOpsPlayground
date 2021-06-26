using System;

namespace Blazor_Server.Data
{
    public class ExportAttribute : Attribute
    {
        public ExportAttribute(bool canExport)
        {
            this.CanExport = canExport;
        }

        public bool CanExport { get; private set; } = false;
    }
}
