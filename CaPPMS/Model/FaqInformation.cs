using System;

namespace CaPPMS.Model
{
    public class FaqInformation
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Question { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Reply { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

    }

}
