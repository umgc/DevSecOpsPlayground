using CaPPMS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaPPMSTests.Data
{
    [TestClass]
    public class ContactTests
    {
        [TestMethod]
        public void ContactInitialization()
        {
            var contact = new Contact();

            foreach(var prop in contact.GetType().GetProperties())
            {
                if(prop.PropertyType != typeof(Guid))
                {
                    Assert.IsNotNull(prop.GetValue(contact));
                }
                else if (prop.PropertyType == typeof(Guid))
                {
                    Assert.IsFalse((Guid)prop.GetValue(contact) == Guid.Empty);
                }
            }
        }
    }
}
