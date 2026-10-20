﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CaPPMS.Data;

namespace CaPPMSTests.Data
{
    [TestClass()]
    public class LocalProjectFilesManagerAdditionalTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            if (ProjectManagerService.BaseDirInfo.Exists)
            {
                foreach (var dirInfo in ProjectManagerService.BaseDirInfo.GetDirectories("*", SearchOption.AllDirectories))
                {
                    try { dirInfo.Delete(true); } catch { }
                }
            }
        }

        #region Path traversal tests

        [TestMethod()]
        public async Task SaveAsync_WithPathTraversal_ThrowsUnauthorizedAccessException()
        {
            var manager = new LocalProjectFilesManager();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("traversal test"));
            var ex = Assert.ThrowsException<AggregateException>(() =>
                Task.Run(async () => await manager.SaveAsync(ms, "../../escape", ".txt")).Result);
            Assert.IsInstanceOfType(ex.InnerException, typeof(UnauthorizedAccessException));
            Assert.IsTrue(ex.InnerException.Message.Contains("Path not within the specified storage location"));
        }

        [TestMethod()]
        public void ReadAsync_WithPathTraversal_ReturnsNull()
        {
            var manager = new LocalProjectFilesManager();
            var result = Task.Run(async () => await manager.ReadAsync("../../../etc/passwd")).Result;
            Assert.IsNull(result);
        }

        [TestMethod()]
        public async Task DeleteAsync_WithUnauthenticatedUser_ReturnsNotAuthenticated()
        {
            var manager = new LocalProjectFilesManager();
            // GenericIdentity with empty name is unauthenticated (IsAuthenticated == false)
            var principal = new GenericPrincipal(new GenericIdentity(""), new string[0]);
            var result = await manager.DeleteAsync("somefile.txt", principal);
            Assert.AreEqual("User is not authenticated", result);
        }

        [TestMethod()]
        public async Task DeleteAsync_WithPathTraversal_ReturnsError()
        {
            var manager = new LocalProjectFilesManager();
            var principal = new GenericPrincipal(new GenericIdentity("test@greatTest.com"), new string[] { "Global Administrator" });
            var result = await manager.DeleteAsync("../../../escape.txt", principal);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains("Path not within the specified storage location"));
        }

        [TestMethod()]
        public void DeleteAsync_FromTest_WithFileOutsideBaseDir_ReturnsAccessDenied()
        {
            var manager = new LocalProjectFilesManager();
            var outsideFile = Path.Combine(Path.GetTempPath(), $"cappms-outside-{Guid.NewGuid():N}.txt");
            File.WriteAllText(outsideFile, "should not be deletable");
            try
            {
                var result = Task.Run(async () => await manager.DeleteAsync(outsideFile)).Result;
                Assert.IsFalse(string.IsNullOrEmpty(result));
                Assert.IsTrue(result.Contains("File access denied"));
            }
            finally
            {
                if (File.Exists(outsideFile)) File.Delete(outsideFile);
            }
        }

        #endregion

        #region Save/Read round-trip tests

        [TestMethod()]
        public async Task SaveAsync_OverwritesExistingFile()
        {
            var manager = new LocalProjectFilesManager();
            var fileId = Guid.NewGuid().ToString();
            var location1 = await manager.SaveAsync(new MemoryStream(Encoding.UTF8.GetBytes("first")), fileId, ".txt");
            var location2 = await manager.SaveAsync(new MemoryStream(Encoding.UTF8.GetBytes("second")), fileId, ".txt");
            Assert.AreEqual(location1, location2);
            using var readStream = await manager.ReadAsync(location2);
            Assert.IsNotNull(readStream);
            using var ms = new MemoryStream();
            readStream.CopyTo(ms);
            var content = Encoding.UTF8.GetString(ms.ToArray());
            Assert.AreEqual("second", content);
        }

        #endregion

        #region Compression tests

        [TestMethod()]
        public void SaveAsync_WritesCompressedFileOnly()
        {
            var manager = new LocalProjectFilesManager();
            var location = Task.Run(async () => await manager.SaveAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("compressed content")), Guid.NewGuid().ToString(), ".txt")).Result;
            var fullPath = Path.Combine(manager.FileDirInfo.FullName, location);
            Assert.IsTrue(File.Exists(fullPath + ".gz"));
            Assert.IsFalse(File.Exists(fullPath));
        }

        #endregion
    }
}
