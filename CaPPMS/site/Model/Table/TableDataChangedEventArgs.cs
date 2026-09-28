using System;
using System.Collections.Generic;

namespace CaPPMS.Model.Table
{
    public class TableDataChangedEventArgs<T> : EventArgs
    {
        public TableDataChangedEventArgs(IEnumerable<T> data)
        {
            this.Data = data;
        }

        public IEnumerable<T> Data { get; private set; }
    }
}
