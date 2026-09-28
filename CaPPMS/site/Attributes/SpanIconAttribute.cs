using System;

namespace CaPPMS.Attributes
{
    public class SpanIconAttribute : Attribute
    {
        public SpanIconAttribute(string iconClass)
        {
            this.IconClass = iconClass;
        }

        public SpanIconAttribute(string iconClass, bool useAsHyperlink)
            : this(iconClass)
        {
            this.UseAsHyperLink = useAsHyperlink;
        }

        public string IconClass { get; set; } = string.Empty;

        public bool UseAsHyperLink { get; set; }
    }
}
