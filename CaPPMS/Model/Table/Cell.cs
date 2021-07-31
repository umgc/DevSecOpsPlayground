using CaPPMS.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

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

        public Cell(int rowId, int columnId, object value, CellType cellType, IEnumerable<Attribute> attributes)
            : this(rowId, columnId, value, cellType)
        {
            this.Attributes = attributes;
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

        public IEnumerable<Attribute> Attributes { get; private set; }

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

            html += ">";

            SpanIconAttribute spanIcon = Attributes.FirstOrDefault(a => a.GetType() == typeof(SpanIconAttribute)) as SpanIconAttribute;
            bool handledValue = false;

            if (spanIcon != null && !string.IsNullOrEmpty(spanIcon.IconClass) & !string.IsNullOrEmpty(cellData))
            {
                string spanData = string.Empty;
                if (spanIcon.UseAsHyperLink)
                {
                    spanData += $"<a class=\"btn btn-secondary\" href=\"{cellData}\">";
                    handledValue = true;
                }

                spanData += $"<span class=\"{spanIcon.IconClass}\"></span>";

                if (spanIcon.UseAsHyperLink)
                {
                    spanData += $"{cellData.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries).Last()}</a>";
                }

                html += spanData;
            }

            // TODO: [rwilson127 - 4 July 21]We can probably expand this to include any url within the who string not just if the string is a url.
            if (!handledValue && Uri.TryCreate(cellData, UriKind.Absolute, out Uri uri))
            {
                html += $"<a href=\"{uri}\">{uri}</a>";
            }
            else if(!handledValue)
            {
                html += cellData;
            }

            html += CellType == CellType.Data ? "</td>" : "</th>";

            return html;
        }
    }
}
