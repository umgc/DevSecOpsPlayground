﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;
using Newtonsoft.Json;
using CaPPMS.Model;
using CaPPMS.Data;

namespace CaPPMSTests.Data.Table
{
    [TestClass()]
    public class ProjectManagerServiceTests
    {
        private const string deleteRole = "Global Administrator";

        [TestInitialize]
        public void Initialize()
        {
            var projectFilePath = this.GetProjectFilePath();

            if (File.Exists(projectFilePath))
            {
                File.Delete(projectFilePath);
            }
        }

        [TestMethod()]
        public void Add()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService();
            Assert.IsTrue(Task.Run(async () => await projectManagerService.AddAsync(CreateIdea())).Result);
            Assert.AreEqual(1, projectManagerService.GetIdeaTitles().Count());
        }

        [TestMethod()]
        public void Remove()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService();
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
        public void FileDidSave()
        {
            ProjectManagerService projectManagerService = new ProjectManagerService();
            Task.Run(async () => await projectManagerService.AddAsync(CreateIdea())).Wait();

            // Wait on second to over come flaky test syndrome.
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            var filePath = GetProjectFilePath();

            Assert.IsTrue(File.Exists(filePath));

            var fileData = File.ReadAllText(filePath);

            var projectData = JsonConvert.DeserializeObject<Dictionary<string, ProjectInformation>>(fileData);
            Assert.AreEqual(1, projectData.Count);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete projects file.
            string filePath = GetProjectFilePath();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Now delete any temp files.
            filePath = filePath + ".temp";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private ProjectInformation CreateIdea()
        {
            var idea = new ProjectInformation
            {
                FirstName = "InitialTestFirstName",
                LastName = "InitialTestLastName",
                IsSponsor = false,
                ProjectTitle = $"InitialTestProjectTitle-{DateTime.Now}",
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

        private string GetProjectFilePath()
        {
            return Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "projects.json");
        }
    }
}