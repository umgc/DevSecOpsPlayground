using AngleSharp.Dom;
using Bunit;
using CaPPMSTests.ComponentTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.Pages
{
    [TestClass]
    public class IndexTests : BunitTestHelper
    {
        [TestMethod]
        public void LoadIndexPage()
        {
            var cut = RenderComponent<CaPPMS.Pages.Index>();

            // Make sure simple form elements are present. It will fail the element cannot be found.

            var testableElements = new[]
            {
                "firstName",
                "lastName",
                "email",
                "phone",
                "projectTitle",
                "projectDescription"
            };

            foreach(var testElement in testableElements)
            {
                Assert.IsNotNull(cut.Find($"#{testElement}"), $"Element: {testElement}. Not found.");
            }
        }
    }
}
