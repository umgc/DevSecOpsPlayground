using CaPPMS.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace CaPPMSTests.Attribute
{
    [TestClass]
    public class ExportAttributeTests
    {
        [TestMethod]
        public void ExportAttributeDefaultIsFalse()
        {
            var dummy = new DummyTableObject();

            var prop = dummy.GetType().GetProperty("Column3");
            Assert.IsFalse(prop?.GetCustomAttribute<ExportAttribute>()?.CanExport ?? false);
        }
    }
}
