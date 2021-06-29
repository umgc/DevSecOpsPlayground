using System;

namespace Blazor_Server.Attributes
{
    public class ExportAttribute : Attribute
    {
        public ExportAttribute() { }

        public ExportAttribute(bool canExport)
        {
            this.CanExport = canExport;
        }

        public bool CanExport { get; private set; } = false;
    }
}
