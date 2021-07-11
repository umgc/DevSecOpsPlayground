using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CaPPMS.Model.Table
{
    public class Row : IList<Cell>
    {
        public Row() { }

        public Row(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }

        public Row(int rowNumber, List<Cell> cells)
            : this(rowNumber)
        {
            this.Cells = cells;
        }

        public int RowNumber { get; private set; } = -1;

        public List<Cell> Cells { get; set; } = new List<Cell>();

        public string CssClasses { get; set; } = string.Empty;

        public bool EnableElementStyle { get; set; } = false;

        public bool AlternatingRowColors { get; set; } = false;

        public Color PrimaryRowColor { get; set; } = Color.White;

        public Color PrimaryRowFontColor { get; set; } = Color.Black;

        public Color? AlternateRowColor { get; set; } = null;

        public Color? AlternateRowFontColor { get; set; } = null;

        public int Count => Cells.Count;

        public bool IsReadOnly => false;

        public object DataBoundItem { get; set; }

        public Cell this[int index]
        {
            get
            {
                return this.Cells[index];
            }
            set
            {
                this.Cells[index] = value;
            }
        }

        public bool ContainsFilter(string filter)
        {
            return string.IsNullOrEmpty(filter) || (!string.IsNullOrEmpty(filter) && this.Cells.Any(c => c.Value.ToString().ToLower().Contains(filter)));
        }

        #region IList Interface

        public int IndexOf(Cell item)
        {
            return this.Cells.IndexOf(item);
        }

        public void Insert(int index, Cell item)
        {
            this.Cells.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.Cells.RemoveAt(index);
        }

        public void Add(Cell item)
        {
            this.Cells.Add(item);
        }

        public void Clear()
        {
            this.Cells.Clear();
        }

        public bool Contains(Cell item)
        {
            return this.Cells.Contains(item);
        }

        public void CopyTo(Cell[] array, int arrayIndex)
        {
            this.Cells.CopyTo(array, arrayIndex);
        }

        public bool Remove(Cell item)
        {
            return this.Remove(item);
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return this.Cells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this .Cells.GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            string html = "<tr";

            if (!string.IsNullOrEmpty(this.CssClasses))
            {
                html += $" class={this.CssClasses}";
            }

            if (EnableElementStyle)
            {
                var fontColor = AlternatingRowColors && AlternateRowFontColor != null ? AlternateRowFontColor.Value.ToArgb() : PrimaryRowFontColor.ToArgb();
                var backgroundColor = AlternatingRowColors && AlternateRowColor != null ? AlternateRowColor.Value.ToArgb() : PrimaryRowColor.ToArgb();
                string style = $" style=\"color:{fontColor}; background-color:{backgroundColor}\"";

                html += style;
            }

            html += ">";
            html += string.Join($"\t\t{Environment.NewLine}", this.Cells);
            html += "</tr>";

            return html;
        }
    }
}
