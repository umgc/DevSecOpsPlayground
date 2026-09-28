using System.Collections.Generic;

namespace CaPPMSTests.Model.Table
{
    public class IEnumberableDummyObject : List<DummyTableObject>
    {
        public IEnumberableDummyObject()
        {
            this.Add(new DummyTableObject());
        }
    }
}
