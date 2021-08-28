using CaPPMS.Attributes;
using System.ComponentModel;

namespace CaPPMSTests
{
    public class DummyTableObject
    {
        [ColumnHeader]
        public string Column1 { get; } = "Row 1 Cell 1";

        [ColumnHeader]
        [DisplayName("Column 2")]
        public string Column2 { get; } = "Row 1 Cell 2";

        [Export]
        public string Column3 { get; } = "Column3";
    }
}
