using Bunit;
using CaPPMSTests.ComponentTests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.ComponentTests
{
    [TestClass]
    public class MyRatingsTests : BunitTestHelper
    {
        [TestMethod]
        public void MyRatingsRendersWithoutCrashingOnEmptyScores()
        {
            // Arrange: Register a DBOperationsService. Even if the database call
            // fails, the component must not crash on an empty studentScores list.
            TestContext.Services.AddSingleton(new CaPPMS.Data.DBOperationsService(
                "Data\\StudentReviews-test.db",
                NullLogger<CaPPMS.Data.DBOperationsService>.Instance));

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.StudentReviews.MyRatings>();

            // Assert: Component renders without throwing.
            var header = cut.Find("h5");
            Assert.IsNotNull(header);
        }
    }
}
