using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class FaqInformation
    {
        //internal static IEnumerable<FaqInformation> Values;

        //public FaqInformation() { }

        public Guid Guid { get; } = Guid.NewGuid();

        public string Question { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Reply { get; set; } = string.Empty;

        public string Topic { get; set; } = string.Empty;

    }

}