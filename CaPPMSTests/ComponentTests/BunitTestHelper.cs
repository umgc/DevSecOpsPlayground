using Bunit;
using CaPPMS.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
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

            // IConfiguration needed once the Consent Form logic can be mocked
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //.AddJsonFile("appsettings-test.json")
            //.Build();

            //TestContext.Services.AddSingleton<IConfiguration>(configuration);
            TestContext.Services.AddSingleton(new ProjectManagerService("Component.tests.json"));
            TestContext.Services.AddOptions();
            TestContext.Services.AddRazorPages()
                  .AddMicrosoftIdentityUI();

            // Causing RemoteJSRuntime ILogger errors in
            // unit tests where authentication is required.

            //TestContext.Services.AddServerSideBlazor()
            //    .AddMicrosoftIdentityConsentHandler();

            TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
        }

        [TestCleanup]
        public void TearDown() => TestContext?.Dispose();
    }
}
