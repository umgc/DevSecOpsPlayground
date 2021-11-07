using CaPPMS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CaPPMSTests.Model
{
    [TestClass]
    public class ContactTests
    {
        [TestMethod]
        public void contactInitialization()
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
