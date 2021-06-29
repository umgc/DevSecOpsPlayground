using System;

namespace Blazor_Server.Attributes
{
    public class ColumnHeaderAttribute :Attribute
    {
        public bool ColumnHeader { get; } = true;
    }
}
