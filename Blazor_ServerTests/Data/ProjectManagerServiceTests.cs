using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;
using Newtonsoft.Json;

namespace Blazor_Server.Data.Tests
{
    [TestClass()]
    public class ProjectManagerServiceTests
    {
        [TestMethod()]
        public void Add()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService();
            Task.Run(async () => await projectManagerService.AddAsync(CreateIdea())).Wait();
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());
        }

        [TestMethod()]
        public void Remove()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService();
            var idea = CreateIdea();

            Task.Run(async () => await projectManagerService.AddAsync(idea)).Wait();
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());

            IPrincipal principal = new GenericPrincipal(new GenericIdentity("test@greatTest.com"), new string[] { "owner" });

            Task.Run(async () => await projectManagerService.RemoveAsync(idea, principal)).Wait();
            Assert.AreEqual(0, projectManagerService.GetIdeaTitles().Count());
        }

        [TestMethod]
        public void FileDidSave()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService();
            Task.Run(async () => await projectManagerService.AddAsync(CreateIdea())).Wait();

            // Wait on second to over come flaky test syndrome.
            Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(5))).Wait();

            var filePath = Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "projects.json");

            Assert.IsTrue(File.Exists(filePath));

            var fileData = File.ReadAllText(filePath);

            var projectData = JsonConvert.DeserializeObject<Dictionary<string, Tuple<IdeaFormModel, HashSet<string>>>>(fileData);
            Assert.AreEqual(1, projectData.Count);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var filePath = Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "projects.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private IdeaFormModel CreateIdea()
        {
            var idea = new IdeaFormModel
            {
                FirstName = "InitialTestFirstName",
                LastName = "InitialTestLastName",
                IsSponsor = false,
                ProjectTilte = $"InitialTestProjectTitle-{DateTime.Now}",
                ProjectDescription = "InitialTestDescription",
                Email = "Initial@test.com",
                Phone = "555-555-8378",
                Url = "www.findaproject.com",
                SponsorEmail = "Sponsor@greatTests.com",
                SponsorFirstName = "SponsorInitialFirstname",
                SponsorLastName = "SponsorInitialLastname",
                SponsorPhone = "555-555-8379",
                SponsorUrl = "www.sponsorsrus.io"
            };

            return idea;
        }
    }
}