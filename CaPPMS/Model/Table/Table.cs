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
    /// <summary>
    /// This table uses reflection to dynamically build rows and cells. Two attributes are used to do this.
    /// 1. The ColumnHeaderAttribute is used to generate the column headers which are used to generate the rows and their cells.
    ///     To add a column, simply decrorate with [ColumnHeader]
    /// 2. The rows will look preference DisplayNameAttribute over the Property Name. DisplayNameAttribute is not required for the table to build.
    /// </summary>
    public class Table : ComponentBase, IList<Row>
    {
        public event EventHandler DataSourceChanged;
        public event EventHandler RowsPerPageChanged;
        public event EventHandler FilterChanged;

        public readonly int[] RowsPerPageOptions = new int[] { 10, 25, 50, 100 };

        private int rowsPerPage = 5;

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

        public List<Row> Rows => this.GetRows().ToList();

        public int Count => this.DataSource.Count();

        public int ColumnCount => this.GetHeaderRow().Count;

        public bool IsReadOnly => false;

        public int CurrentPage { get; private set; } = 1;

        public int SortColumnIndex { get; set; } = 0;
        public bool IsColumnSortAscending { get; set; } = true;

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
            this.StateHasChanged();
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

        public IEnumerable<Row> GetRows()
        {
            var dataList = this.dataSource.ToList();

            List<Row> rows = new List<Row>();
            for (int r = 0; r < dataList.Count; r++)
            {
                Row row = new Row(r);
                for (int c = 0; c < this.HeaderRow.Count; c++)
                {
                    var prop = dataList[r].GetType().GetRuntimeProperties()
                                .FirstOrDefault(p => (p.GetCustomAttribute(typeof(DisplayNameAttribute)) != null &&
                                p.GetCustomAttribute<DisplayNameAttribute>().DisplayName == this.HeaderRow[c].Value as string) ||
                                p.Name == this.HeaderRow[c].Value as string);

                    if (prop is null)
                    {
                        row.Add(new Cell(r, c, string.Empty));
                    }
                    else
                    {
                        row.Add(new Cell(r, c, prop.GetValue(dataList[r]), CellType.Data, prop.GetCustomAttributes()));
                    }
                }

                if (row.ContainsFilter(filter))
                {
                    row.DataBoundItem = dataList[r];
                    rows.Add(row);
                }
            }

            if (IsColumnSortAscending)
            {
                rows = rows.OrderBy(o => o.Cells[SortColumnIndex]).ToList();
            } else
            {
                rows = rows.OrderByDescending(o => o.Cells[SortColumnIndex]).ToList();
            }

            int skipNumber = CurrentPage > 1 ? (CurrentPage * rowsPerPage) - rowsPerPage : 0;
            return rows.Skip(skipNumber - 1).Take(this.rowsPerPage).ToArray();
        }

        public void SetDataSource(IEnumerable<object> dataSource)
        {
            this.DataSource = dataSource;
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
    }
}
