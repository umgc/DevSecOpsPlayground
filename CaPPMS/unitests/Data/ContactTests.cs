using CaPPMS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
                    if (prop.GetValue(contact) is Guid contactId
                        && contactId == Guid.Empty)
                    {
                        throw new InvalidOperationException("Guid is empty.");
                    }
                }
            }
        }
    }
}
