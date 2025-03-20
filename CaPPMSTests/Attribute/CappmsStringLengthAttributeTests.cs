using CaPPMS.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CaPPMSTests.Attribute
{
    [TestClass]
    public class CappmsStringLengthAttributeTests
    {
        [TestMethod]
        public async Task StringIsValid()
        {
            // Arrange
            var attribute = new CappmsStringLengthAttribute(100)
            {
                MinimumLength = 5,
            };

            string value = "This is a test string.";
            Assert.IsTrue(attribute.IsValid(value));
            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task StringIsInvalidMinLen()
        {
            // Arrange
            var attribute = new CappmsStringLengthAttribute(100)
            {
                MinimumLength = 5,
            };
            string value = "Test";
            Assert.IsFalse(attribute.IsValid(value));
            string errorMessage = attribute.FormatErrorMessage("Test");
            Assert.AreEqual("The field Test must be a string with a minimum length of 5 and a maximum length of 100. Current length:4.", errorMessage);

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task StringIsInvalidMaxLen()
        {
            // Arrange
            var attribute = new CappmsStringLengthAttribute(10)
            {
                MinimumLength = 5,
            };
            string value = "This is a test string.";
            Assert.IsFalse(attribute.IsValid(value));
            string errorMessage = attribute.FormatErrorMessage("Test");
            Assert.AreEqual("The field Test must be a string with a minimum length of 5 and a maximum length of 10. Current length:22.", errorMessage);
            await Task.CompletedTask;
        }
    }
}
