using CaPPMS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaPPMSTests.Model
{
    /// <summary>
    /// Test the FaqInformation class functionality
    /// </summary>
    [TestClass]
    public class FaqInformationTests
    {
        /// <summary>
        /// Test if the fields are properly initialized
        /// </summary>
        [TestMethod]
        public void TestInitialization()
        {
            FaqInformation faqInformation = new FaqInformation();

            foreach (var property in faqInformation.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(Guid))
                { 
                    Assert.AreNotEqual(property.GetValue(faqInformation), Guid.Empty);
                }
                else
                {
                    Assert.IsNotNull(property.GetValue(faqInformation));
                }
            }
        }
    }
}
