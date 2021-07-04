using System;
using System.Drawing;

namespace CaPPMS.Model.Table
{
    public enum CellType
    {
        Header,
        Data
    }

    public class Cell
    {
        public Cell() { }

        public Cell(int rowId, int columnId, object value)
        {
            this.RowId = rowId;
            this.ColumnId = columnId;
            this.Value = value;
        }

        public Cell(int rowId, int columnId, object value, CellType cellType)
            : this(rowId, columnId, value)
        {
            this.CellType = cellType;
        }

        public CellType CellType { get; private set; } = CellType.Data;

        public object Value { get; set; }

        public int RowId { get; private set; }

        public int ColumnId { get; private set; }

        public string CssClasses { get; set; } = string.Empty;

        public bool EnableElementStyle { get; set; } = false;

        public Color PrimaryRowColor { get; set; } = Color.White;

        public Color PrimaryRowFontColor { get; set; } = Color.Black;

        public int ColSpan { get; set; } = 1;

        public override string ToString()
        {
            string html = CellType == CellType.Data ? "<td" : "<th";

            if (ColSpan > 1)
            {
                html += $" colspan=\"{ColSpan}\"";
            }

            if (!string.IsNullOrEmpty(this.CssClasses))
            {
                html += $" class={this.CssClasses}";
            }

            if (EnableElementStyle)
            {
                string style = $" style=\"color:{PrimaryRowFontColor.ToArgb()}; background-color:{PrimaryRowColor.ToArgb()}\"";

                html += style;
            }

            string cellData = Value.ToString();

            // TODO: [rwilson127 - 4 July 21]We can probably expand this to include any url within the who string not just if the string is a url.
            if (Uri.TryCreate(cellData, UriKind.Absolute, out Uri uri))
            {
                cellData = $"<a href=\"{uri}\">{uri}</a>";
            }

            html += ">";
            html += cellData;
            html += CellType == CellType.Data ? "</td>" : "</th>";

            return html;
        }
    }
}
