using Bunit;
using CaPPMS.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.ComponentTests
{
    public abstract class BunitTestHelper : TestContextWrapper
    {
        [TestInitialize]
        public void Setup()
        {
            TestContext = new Bunit.TestContext();

            // Register services
            TestContext.Services.AddSingleton(new ProjectManagerService());
            TestContext.Services.AddOptions();
            TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [TestCleanup]
        public void TearDown() => TestContext?.Dispose();
    }
}
