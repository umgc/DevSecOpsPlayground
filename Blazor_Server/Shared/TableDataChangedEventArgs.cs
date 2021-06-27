using System;
using System.Collections.Generic;

namespace Blazor_Server.Shared
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
