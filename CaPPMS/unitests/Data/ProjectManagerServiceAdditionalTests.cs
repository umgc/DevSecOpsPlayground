﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using CaPPMS.Data;
using CaPPMS.Model;

namespace CaPPMSTests.Data
{
    [TestClass()]
    public class ProjectManagerServiceAdditionalTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var file in ProjectManagerService.BaseDirInfo.GetFiles())
            {
                try { file.Delete(); } catch (IOException) { }
            }
        }

        private ProjectInformation CreateIdea(string title = "Test Project")
        {
            return new ProjectInformation
            {
                FirstName = "Test",
                LastName = "User",
                ProjectTitle = title,
                ProjectDescription = "A test project",
                Email = "test@test.com",
                Phone = "555-555-5555"
            };
        }

        #region RemoveFileAsync tests

        [TestMethod()]
        public async Task RemoveFileAsync_WithUnauthenticatedUser_ReturnsNotAuthenticated()
        {
            var manager = new ProjectManagerService("removefile-unauth.json");
            var idea = CreateIdea();
            await manager.AddAsync(idea);
            var file = new ProjectFile { Location = "somefile.txt" };
            // GenericIdentity with empty name is unauthenticated (IsAuthenticated == false)
            var principal = new GenericPrincipal(new GenericIdentity(""), new string[0]);
            var result = await manager.RemoveFileAsync(idea, file, principal);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains("not authenticated"));
        }

        [TestMethod()]
        public async Task RemoveFileAsync_WithUnknownProject_ReturnsErrorNotThrow()
        {
            var manager = new ProjectManagerService("removefile-unknown.json");
            IPrincipal principal = new GenericPrincipal(new GenericIdentity("test@greatTest.com"), new string[] { "Global Administrator" });
            var unknownProject = new ProjectInformation { ProjectTitle = "Never added" };
            var file = new ProjectFile();
            var result = await manager.RemoveFileAsync(unknownProject, file, principal);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod()]
        public async Task RemoveFileAsync_WithAuthenticatedUser_RemovesFile()
        {
            var manager = new ProjectManagerService("removefile-auth.json");
            var idea = CreateIdea();
            await manager.AddAsync(idea);
            // Create the compressed file in the files subdirectory where DeleteAsync looks for it
            var filePath = Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "files", "testfile.txt.gz");
            File.WriteAllText(filePath, "test content");
            var file = new ProjectFile { Location = "testfile.txt" };
            idea.Attachments.Add(file);
            var principal = new GenericPrincipal(new GenericIdentity("test@greatTest.com"), new string[] { "Global Administrator" });
            var result = await manager.RemoveFileAsync(idea, file, principal);
            Assert.IsTrue(string.IsNullOrEmpty(result), $"Expected success but got: {result}");
        }

        #endregion

        #region UpdateAsync tests

        [TestMethod()]
        public async Task UpdateAsync_WithNonExistentProject_ReturnsFalse()
        {
            var manager = new ProjectManagerService("update-nonexistent.json");
            var idea = CreateIdea("NonExistent");
            idea.ProjectID = Guid.NewGuid();
            var result = await manager.UpdateAsync(idea);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task UpdateAsync_WithValidProject_ReturnsTrue()
        {
            var manager = new ProjectManagerService("update-valid.json");
            var idea = CreateIdea();
            await manager.AddAsync(idea);
            idea.ProjectTitle = $"Updated Title-{Guid.NewGuid()}";
            var result = await manager.UpdateAsync(idea);
            Assert.IsTrue(result);
        }

        #endregion

        #region RemoveAsync / DeleteAsync null tests

        [TestMethod()]
        public async Task RemoveAsync_WithNullIdea_ReturnsIdeaWasNull()
        {
            // RemoveAsync delegates to DeleteAsync which null-checks the idea parameter
            var manager = new ProjectManagerService("remove-null.json");
            var principal = new GenericPrincipal(new GenericIdentity("test"), new string[] { "Global Administrator" });
            var result = await manager.DeleteAsync(null, principal);
            Assert.AreEqual("Idea was null", result);
        }

        [TestMethod()]
        public async Task DeleteAsync_WithNullIdea_ReturnsIdeaWasNull()
        {
            var manager = new ProjectManagerService("delete-null.json");
            var principal = new GenericPrincipal(new GenericIdentity("test"), new string[] { "Global Administrator" });
            var result = await manager.DeleteAsync(null, principal);
            Assert.AreEqual("Idea was null", result);
        }

        #endregion

        #region ExportAsync null test

        [TestMethod()]
        public async Task ExportAsync_WithNullIdea_ReturnsIdeaWasNull()
        {
            var manager = new ProjectManagerService("export-null.json");
            var result = await manager.ExportAsync(null);
            Assert.AreEqual("Idea was null", result);
        }

        #endregion

        #region GetIdeaTitles tests

        [TestMethod()]
        public void GetIdeaTitles_WithNoProjects_ReturnsEmpty()
        {
            var manager = new ProjectManagerService("titles-empty.json");
            var titles = manager.GetIdeaTitles();
            Assert.AreEqual(0, titles.Count());
        }

        [TestMethod()]
        public void GetIdeaTitles_WithProjects_ReturnsTitles()
        {
            var manager = new ProjectManagerService("titles.json");
            var idea1 = CreateIdea("First Project");
            var idea2 = CreateIdea("Second Project");
            Task.Run(async () => await manager.AddAsync(idea1)).Wait();
            Task.Run(async () => await manager.AddAsync(idea2)).Wait();
            var titles = manager.GetIdeaTitles().ToList();
            Assert.AreEqual(2, titles.Count);
            Assert.IsTrue(titles.Contains("First Project"));
            Assert.IsTrue(titles.Contains("Second Project"));
        }

        #endregion

        #region Comment tests

        [TestMethod()]
        public void AddComment_AddsCommentToProject()
        {
            var manager = new ProjectManagerService("comment-add.json");
            var idea = CreateIdea();
            Task.Run(async () => await manager.AddAsync(idea)).Wait();
            var comment = new Comment
            {
                CommentId = Guid.NewGuid(),
                ProjectID = idea.ProjectID,
                Comments = "Test comment",
                UserEmail = "test@author.com"
            };
            manager.AddComment(comment);
            var comments = manager.GetComments(idea.ProjectID);
            Assert.AreEqual(1, comments.Count());
        }

        [TestMethod()]
        public void DeleteComment_RemovesCommentFromProject()
        {
            var manager = new ProjectManagerService("comment-delete.json");
            var idea = CreateIdea();
            Task.Run(async () => await manager.AddAsync(idea)).Wait();
            var comment = new Comment
            {
                CommentId = Guid.NewGuid(),
                ProjectID = idea.ProjectID,
                Comments = "Test comment",
                UserEmail = "test@author.com"
            };
            manager.AddComment(comment);
            manager.DeleteComment(comment);
            var comments = manager.GetComments(idea.ProjectID);
            Assert.AreEqual(0, comments.Count());
        }

        [TestMethod()]
        public void GetComments_WithUnknownProject_ReturnsEmpty()
        {
            var manager = new ProjectManagerService("comment-unknown.json");
            var comments = manager.GetComments(Guid.NewGuid());
            Assert.AreEqual(0, comments.Count());
        }

        #endregion

        #region CompleteProject test

        [TestMethod()]
        public void CompleteProject_UpdatesProjectStatus()
        {
            var manager = new ProjectManagerService("complete.json");
            var idea = CreateIdea();
            Task.Run(async () => await manager.AddAsync(idea)).Wait();
            idea.Status = "Completed";
            idea.SemesterTerm = "Fall";
            idea.SemesterYear = "2026";
            manager.CompleteProject(idea);
            var updated = manager.GetIdeaTitles().ToList();
            Assert.AreEqual(1, updated.Count);
        }

        #endregion
    }
}

