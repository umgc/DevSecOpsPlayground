using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;
using Newtonsoft.Json;
using CaPPMS.Model;
using CaPPMS.Data;

namespace CaPPMSTests.Data
{
    [TestClass()]
    public class ProjectManagerServiceTests
    {
        private const string deleteRole = "Global Administrator";

        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestMethod()]
        public void Add()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService("add.json");
            var idea = CreateIdea();
            Assert.IsTrue(Task.Run(async () => await projectManagerService.AddAsync(idea)).Result);
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());

            Assert.IsTrue(idea.ProjectID != Guid.Empty);
        }

        [TestMethod()]
        public void Remove()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService("remove.json");
            var idea = CreateIdea();

            Task.Run(async () => await projectManagerService.AddAsync(idea)).Wait();
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());

            IPrincipal principal = new GenericPrincipal(new GenericIdentity("test@greatTest.com"), new string[] { deleteRole });

            Assert.IsTrue(string.IsNullOrEmpty(Task.Run(async () => await projectManagerService.RemoveAsync(idea, principal)).Result));

            // Need a delay for Async testing
            Task.Delay(TimeSpan.FromSeconds(1)).Wait();

            Assert.AreEqual(0, projectManagerService.GetIdeaTitles().Count());
        }

        [TestMethod]
        public void fileDidSave()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService("savefile.json");
            Task.Run(async () => await projectManagerService.AddAsync(CreateIdea())).Wait();

            // Wait on second to over come flaky test syndrome.
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            var filePath = Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "savefile.json");

            Assert.IsTrue(File.Exists(filePath));

            var fileData = File.ReadAllText(filePath);

            var projectData = JsonConvert.DeserializeObject<Dictionary<string, ProjectInformation>>(fileData);
            Assert.AreEqual(1, projectData.Count);
        }

        [TestMethod]
        public void didUpdateProjectManager()
        {
            string newProjectTitle = $"New Test Project Title-{Guid.NewGuid()}";
            ProjectManagerService projectManagerService = new ProjectManagerService("update.json");
            var idea = CreateIdea();
            Assert.IsTrue(Task.Run(async () => await projectManagerService.AddAsync(idea)).Result);
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());

            idea.ProjectTitle = newProjectTitle;

            Assert.IsTrue(Task.Run(async () => await projectManagerService.UpdateAsync(idea)).Result);

            Assert.AreEqual(newProjectTitle, projectManagerService.GetIdeaTitles().ToList()[0]);
        }

        [TestMethod]
        public void doesExportMatch()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService("add.json");
            var idea = CreateIdea();
            Assert.IsTrue(Task.Run(async () => await projectManagerService.AddAsync(idea)).Result);
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());

            //var path = Task.Run(async () => await projectManagerService.ExportAsync(idea)).Result;

            //Assert.IsTrue(File.Exists(path));

            //Assert.AreEqual("", "");
        }

        private ProjectInformation CreateIdea()
        {
            var idea = new ProjectInformation
            {
                FirstName = "InitialTestFirstName",
                LastName = "InitialTestLastName",
                IsSponsor = false,
                ProjectTitle = $"InitialTestProjectTitle-2021-07-21",
                ProjectDescription = "InitialTestDescription",
                Email = "Initial@test.com",
                Phone = "555-555-8378",
                Url = "www.findaproject.com",
                SponsorEmail = "Sponsor@greatTests.com",
                SponsorFirstName = "SponsorInitialFirstname",
                SponsorLastName = "SponsorInitialLastname",
                SponsorPhone = "555-555-8379"
            };

            return idea;
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete projects file.
            foreach (var file in ProjectManagerService.BaseDirInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (IOException)
                {
                }
            }
        }
    }
}