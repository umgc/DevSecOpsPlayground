using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaPPMSTests.WebDriver
{
    [TestClass]
    public class SubmitIdeaTests
    {
        private const string _url = "https://umgc-cappms.azurewebsites.net";

        private IWebDriver _webDriver;

        [TestInitialize]
        public void Initialize()
        {
            var timeout = TimeSpan.FromSeconds(30);

            var service = ChromeDriverService.CreateDefaultService();
            _webDriver = new ChromeDriver(service, new ChromeOptions { AcceptInsecureCertificates = true }, timeout);
        }

        [TestMethod]
        public void ValidateWebDriver()
        {
            _webDriver.Navigate().GoToUrl(_url);
            Assert.AreEqual("CaPPMS", _webDriver.Title);
        }

        [ClassCleanup]
        public void Cleanup()
        {
            // exit browser session
            _webDriver.Quit();
            _webDriver.Dispose();
        }
    }
}
