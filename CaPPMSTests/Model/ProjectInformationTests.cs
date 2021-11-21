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
    /// Test the ProjectInformationTests class functionality
    /// </summary>
    [TestClass]
    public class ProjectInformationTests
    {
        /// <summary>
        /// Test fields initialization
        /// </summary>
        [TestMethod]
        public void testInitialization()
        {
            ProjectInformation faqInformation = new ProjectInformation();

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

        /// <summary>
        /// test the AddAttachments method and Attachments list
        /// </summary>
        [TestMethod]
        public void testAttachments()
        {
            ProjectInformation projectInformation = new ProjectInformation();

            List<IProjectFile> projectFiles = new List<IProjectFile>() { new ProjectFile(), new ProjectFile() };

            Assert.AreEqual(0, projectInformation.Attachments.Count);

            projectInformation.AddAttachments(projectFiles);
            Assert.AreEqual(projectFiles.Count, projectInformation.Attachments.Count);

            projectInformation.AddAttachments(new List<IProjectFile>() { new ProjectFile() });

            Assert.AreEqual(projectFiles.Count + 1, projectInformation.Attachments.Count);

            projectInformation.SetAttachments(projectFiles);
            Assert.AreEqual(projectFiles.Count, projectInformation.Attachments.Count);
        }
    }
}
