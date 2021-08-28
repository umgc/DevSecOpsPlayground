using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.Model.Table
{
    [TestClass()]
    public class TableTests
    {
        [TestMethod]
        public void NumberOfColumns()
        {
            var table = new CaPPMS.Model.Table.Table()
            {
                DataSource = new IEnumberableDummyObject()
            };

            Assert.AreEqual(2, table.HeaderRow.Count);
        }

        [TestMethod]
        public void ColumnNames()
        {
            var table = new CaPPMS.Model.Table.Table()
            {
                DataSource = new IEnumberableDummyObject()
            };

            Assert.AreEqual("Column1", table.HeaderRow[0].Value.ToString());
            Assert.AreEqual("Column 2", table.HeaderRow[1].Value.ToString());
        }

        [TestMethod]
        public void VerifySingleRow()
        {
            var table = new CaPPMS.Model.Table.Table()
            {
                DataSource = new IEnumberableDummyObject()
            };

            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("Row 1 Cell 1", table.Rows[0][0].Value.ToString());
            Assert.AreEqual("Row 1 Cell 2", table.Rows[0][1].Value.ToString());
        }
    }
}