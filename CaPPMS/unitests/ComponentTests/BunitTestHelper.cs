using Bunit;
using CaPPMS.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudBlazor.Services;

namespace CaPPMSTests.ComponentTests
{
    public abstract class BunitTestHelper : TestContextWrapper
    {
        [TestInitialize]
        public void Setup()
        {
            TestContext = new Bunit.TestContext();

            TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
            TestContext.Services.AddSingleton(new ProjectManagerService("Component.tests.json"));

            // Register MudBlazor services needed by components using MudCard, MudGrid, etc.
            TestContext.Services.AddMudServices();
        }

        [TestCleanup]
        public void TearDown() => TestContext?.Dispose();
    }
}
