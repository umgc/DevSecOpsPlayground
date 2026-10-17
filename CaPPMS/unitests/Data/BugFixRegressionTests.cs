using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using CaPPMS.Data;
using CaPPMS.Model;

namespace CaPPMSTests.Data
{
    /// <summary>
    /// Regression tests for the slice-1 bug fixes: path traversal in file reads,
    /// KeyNotFoundException in RemoveFileAsync, config parsing defaults, and
    /// synchronous project persistence.
    /// </summary>
    [TestClass()]
    public class BugFixRegressionTests
    {
        [TestMethod]
        public void ReadAsyncRejectsPathTraversal()
        {
            var manager = new LocalProjectFilesManager();

            // Create a file OUTSIDE the storage directory to prove it is not reachable.
            var outsideFile = Path.Combine(Path.GetTempPath(), $"cappms-outside-{Guid.NewGuid():N}.txt");
            File.WriteAllText(outsideFile, "should not be readable");

            try
            {
                var relativePath = Path.GetRelativePath(manager.FileDirInfo.FullName, outsideFile);
                Assert.IsFalse(string.IsNullOrEmpty(relativePath), "Test setup failed: expected a relative path");

                Stream? result = Task.Run(async () => await manager.ReadAsync(relativePath)).Result;
                Assert.IsNull(result, "ReadAsync must not resolve paths outside the storage directory.");
            }
            finally
            {
                if (File.Exists(outsideFile))
                {
                    File.Delete(outsideFile);
                }
            }
        }

        [TestMethod]
        public void ReadAsyncReturnsNullForMissingFile()
        {
            var manager = new LocalProjectFilesManager();

            Assert.IsNull(Task.Run(async () => await manager.ReadAsync($"missing-{Guid.NewGuid():N}.txt")).Result);
        }

        [TestMethod]
        public void RemoveFileUnknownProjectDoesNotThrow()
        {
            var manager = new ProjectManagerService("removefileunknown.json");
            IPrincipal principal = new GenericPrincipal(new GenericIdentity("test@greatTest.com"), new string[] { "Global Administrator" });

            // Brand-new project id that was never added to the manager.
            var unknownProject = new ProjectInformation { ProjectTitle = "Never added" };
            var file = new ProjectFile();

            string? result = Task.Run(async () => await manager.RemoveFileAsync(unknownProject, file, principal)).Result;

            Assert.IsFalse(string.IsNullOrEmpty(result), "Removing a file from an unknown project should report an error, not throw.");
        }

        [TestMethod]
        public void FileLimitsFallBackToDefaultsWhenConfigMissing()
        {
            // In unit tests Program has no host properties, so these must resolve to
            // defaults instead of throwing FormatException on Convert.ToInt32("").
            var manager = new ProjectManagerService("limits.json");

            Assert.AreEqual(10, manager.MaxNumberOfFiles);
            Assert.AreEqual(1024L * 1024L * 20, manager.MaxMBSizePerFile);
        }

        [TestMethod]
        public void ProjectPersistsSynchronouslyOnAdd()
        {
            var manager = new ProjectManagerService("syncpersist.json");

            var idea = new ProjectInformation
            {
                FirstName = "Sync",
                LastName = "Test",
                ProjectTitle = $"SyncPersist-{Guid.NewGuid():N}",
                Email = "sync@test.com"
            };

            Task.Run(async () => await manager.AddAsync(idea)).Wait();

            // Persistence is synchronous now: the file must exist immediately, no delay needed.
            var filePath = Path.Combine(ProjectManagerService.BaseDirInfo.FullName, "syncpersist.json");
            Assert.IsTrue(File.Exists(filePath), "projects json must be written synchronously during AddAsync.");
        }
    }
}
