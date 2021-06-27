using System;

namespace Blazor_Server.Data
{
    public class ColumnHeaderAttribute :Attribute
    {
        public bool ColumnHeader { get; } = true;
    }
}
