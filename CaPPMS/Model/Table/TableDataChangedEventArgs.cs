using System;
using System.Collections.Generic;

namespace CaPPMS.Model.Table
{
    public class TableDataChangedEventArgs : EventArgs
    {
        public TableDataChangedEventArgs(IEnumerable<object> data)
        {
            this.Data = data;
        }

        public IEnumerable<object> Data { get; private set; }
    }
}
