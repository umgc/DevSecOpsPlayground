using System;

namespace CaPPMS.Attributes
{
    public class ColumnHeaderAttribute :Attribute
    {
        public bool ColumnHeader { get; } = true;
    }
}
