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
    /// Testing the funcitonlaity of the comment class
    /// </summary>
    [TestClass]
    public class CommentTests 
    {
        /// <summary>
        /// Testing the intiilization of the Comment
        /// </summary>
        [TestMethod]
        public void commentInitialization()
        {
            var projectID = Guid.NewGuid();
            var comment = new Comment(projectID);

            foreach (var prop in comment.GetType().GetProperties())
            {
                if (prop.PropertyType != typeof(Guid))
                {
                    Assert.IsNotNull(prop.GetValue(comment));
                }
                else if (prop.PropertyType == typeof(Guid))
                {
                    Assert.IsFalse((Guid)prop.GetValue(comment) == Guid.Empty);
                }
            }
            Assert.AreEqual(projectID, comment.ProjectID);
        }
    }
}
