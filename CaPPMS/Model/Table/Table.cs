using CaPPMS.Attributes;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CaPPMS.Model.Table
{
    public class Table : ComponentBase, IList<Row>
    {
        public event EventHandler DataSourceChanged;
        public event EventHandler RowsPerPageChanged;
        public event EventHandler FilterChanged;

        public readonly int[] RowsPerPageOptions = new int[] { 10, 25, 50, 100 };

        private int rowsPerPage = 10;

        public Table()
        {
        }

        public Row this[int index]
        {
            get
            {
                return this.Rows[index];
            }
            set
            {
                this.Rows[index] = value;
            }
        }

        private IEnumerable<object> dataSource;

        [Parameter]
        public IEnumerable<object> DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (value is IEnumerable<object>)
                {
                    this.dataSource = value;
                    DataSourceChanged?.Invoke(this, new TableDataChangedEventArgs(this.dataSource));
                }
            }
        }

        [Parameter]
        public int RecordsPerPage
        {
            get
            {
                return this.rowsPerPage;
            }
            set
            {
                this.SetRowsPerPage(value);
            }
        }

        private string filter = string.Empty;

        public string Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                this.filter = value;
                this.FilterChanged?.Invoke(this.filter, EventArgs.Empty);
            }
        }

        public Row HeaderRow => GetHeaderRow();

        public string HeaderHtml => $"<thead class=\"{string.Join(" ", HeaderCssClasses)}\">{this.HeaderRow}</thead>";

        public List<string> HeaderCssClasses { get; set; } = new List<string>();

        public List<Row> Rows => this.GetRows().ToList();

        public List<string> BodyCssClasses { get; } = new List<string>();

        public string BodyHtml
        {
            get
            {
                string rows = string.Join($"\t\t\t{Environment.NewLine}", this.GetRows());
                return $"<tbody class=\"{string.Join(" ", BodyCssClasses)}\">{rows}</tbody>";
            }
        }

        public Row FooterRow => GetFooterRow();

        public List<string> FooterCssClasses { get; } = new List<string>();

        public string FooterHtml => $"<tfoot class=\"{string.Join(" ", FooterCssClasses)}\">{this.FooterRow}</tfoot>";

        public List<string> TableCssClasses { get; } = new List<string>();

        public int Count => this.DataSource.Count();

        public int ColumnCount => this.GetHeaderRow().Count;

        public bool IsReadOnly => false;        

        public int CurrentPage { get; private set; } = 1;

        #region IList Interface

        public void Add(Row item)
        {
            this.Rows.Add(item);
        }

        public void Clear()
        {
            this.Rows.Clear();
        }

        public bool Contains(Row item)
        {
            return this .Rows.Contains(item);
        }

        public void CopyTo(Row[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Row> GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }

        public int IndexOf(Row item)
        {
            return this.Rows.IndexOf(item);
        }

        public void Insert(int index, Row item)
        {
            this.Rows.Insert(index, item);
        }

        public bool Remove(Row item)
        {
            return this.Rows.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.Rows.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }

        #endregion

        public void SetRowsPerPage(int rowsPerPage)
        {
            this.rowsPerPage = rowsPerPage;
            this.RowsPerPageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetPageNumber(int page)
        {
            CurrentPage = page;
        }

        public Row GetHeaderRow()
        {
            var row = new Row();
            List<string> cNames = new List<string>(GetColumnNames());

            for(int c = 0; c < cNames.Count; c++)
            {
                row.Add(new Cell(-1 , c, cNames[c], CellType.Header));
            }

            return row;
        }

        public void SetDataSource(IEnumerable<object> dataSource)
        {
            this.DataSource = dataSource;
        }

        public override string ToString()
        {
            string html = "<table";

            if (this.TableCssClasses.Count > 0)
            {
                html += $" class={string.Join(" ", this.TableCssClasses)}";
            }

            html += ">";

            html += this.HeaderHtml;
            html += this.BodyHtml;
            html += this.FooterHtml;

            html += "</table>";

            return html;
        }

        private List<string> GetColumnNames()
        {
            List<string> columns = new List<string>();

            if (this.dataSource.Count() < 1)
            {
                columns.Add("No Data");
                return columns;
            }

            foreach (var prop in this.dataSource.First().GetType().GetRuntimeProperties())
            {
                var attribute = prop.GetCustomAttribute<ColumnHeaderAttribute>();
                if (attribute is null)
                {
                    continue;
                }

                if (attribute.ColumnHeader)
                {
                    string displayName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;
                    columns.Add(displayName);
                }
            }

            return columns;
        }

        private IEnumerable<Row> GetRows()
        {
            int skipNumber = CurrentPage > 1 ? CurrentPage * rowsPerPage : 0;
            var dataList = this.dataSource.Skip(skipNumber).Take(this.rowsPerPage).ToArray();

            for (int r = 0; r < dataList.Length; r++)
            {
                Row row = new Row(r);
                for (int c = 0; c < this.HeaderRow.Count; c++)
                {
                    var prop = dataList[r].GetType().GetRuntimeProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(DisplayNameAttribute)) != null &&
                                                                                        p.GetCustomAttribute<DisplayNameAttribute>().DisplayName == this.HeaderRow[c].Value as string);

                    if (prop is null)
                    {
                        break;
                    }
                    else
                    {
                        row.Add(new Cell(r, c, prop.GetValue(dataList[r])));
                    }
                }

                if (row.ContainsFilter(filter))
                {
                    yield return row;
                }
            }
        }

        private Row GetFooterRow()
        {
            // We will build out the pagination values.
            // The cell <td></td> will be wrapped around it so not needed here.
            // Keep it simple and make it easy.
            // <- pagenumber -> Number per page

            var initialNumberOfPages = this.Count / this.rowsPerPage;
            var numberOfPages = initialNumberOfPages % 2 == 0 ? (int)initialNumberOfPages : (int)Math.Round((double)initialNumberOfPages, MidpointRounding.ToNegativeInfinity) + 1;
            var leftArrowDisabled = CurrentPage > 1 ? string.Empty : "disabled";
            var rightArrowDisabled = CurrentPage < numberOfPages ? string.Empty : "disabled";

            var htmlFooter = $"<button class=\"rounded\" type=\"button\" {leftArrowDisabled}><span class=\"fas fa-arrow-left\"></span></button>";
            htmlFooter += $"&nbsp;{CurrentPage} / {numberOfPages}&nbsp;";
            htmlFooter += $"<button class=\"rounded\" type=\"button\" @onclick=\"PageRight\" {rightArrowDisabled}><span class=\"fas fa-arrow-right\"></span></button>";

            // Add html to cell and set to span across all columns
            var cell = new Cell(-2, 0, htmlFooter);
            cell.ColSpan = this.ColumnCount;
            cell.Value = htmlFooter;
            var row = new Row(-2);
            row.Add(cell);
            return row;
        }
    }
}
