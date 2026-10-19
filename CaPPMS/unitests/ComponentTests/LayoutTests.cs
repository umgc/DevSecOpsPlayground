using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.ComponentTests
{
    [TestClass]
    public class LayoutTests
    {
        private static readonly string HostPath =
            Path.Combine(AppContext.BaseDirectory, "../../../../../CaPPMS/site/Pages/_Host.cshtml");

        [TestMethod]
        public void HostIncludesMudBlazorCss()
        {
            // Arrange: Read the _Host.cshtml layout file.
            var hostContent = File.ReadAllText(HostPath);

            // Act/Assert: Verify MudBlazor CSS is referenced.
            Assert.IsTrue(hostContent.Contains("_content/MudBlazor/MudBlazor.min.css"),
                "_Host.cshtml should reference MudBlazor.min.css for component styling.");
        }

        [TestMethod]
        public void HostIncludesBlazorFrameworkScript()
        {
            // Arrange: Read the _Host.cshtml layout file.
            var hostContent = File.ReadAllText(HostPath);

            // Act/Assert: Verify Blazor server script is referenced.
            Assert.IsTrue(hostContent.Contains("_framework/blazor.server.js"),
                "_Host.cshtml should reference the Blazor server framework script.");
        }

        [TestMethod]
        public void HostIncludesSiteCss()
        {
            // Arrange: Read the _Host.cshtml layout file.
            var hostContent = File.ReadAllText(HostPath);

            // Act/Assert: Verify site.css is referenced.
            Assert.IsTrue(hostContent.Contains("css/site.css"),
                "_Host.cshtml should reference site.css for application styling.");
        }

        [TestMethod]
        public void HostIncludesBootstrapCss()
        {
            // Arrange: Read the _Host.cshtml layout file.
            var hostContent = File.ReadAllText(HostPath);

            // Act/Assert: Verify Bootstrap CSS is referenced.
            Assert.IsTrue(hostContent.Contains("css/bootstrap/bootstrap.min.css"),
                "_Host.cshtml should reference Bootstrap CSS.");
        }
    }
}
