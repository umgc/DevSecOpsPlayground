using System;
using System.Threading.Tasks;

namespace CaPPMS.Model.Table
{
    public class CustomCell<T>
    {
        public string CssClasses { get; set; } = string.Empty;

        public Func<T, object>? Value { get; set; }

        public string Header { get; set; } = string.Empty;

        public Func<T, Task<T?>> OnClick { get; set; } = default!;

        public bool IsVisible { get; set; } = true;
    }
}
