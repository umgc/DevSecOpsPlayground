using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CaPPMS.Data;

namespace CaPPMSTests.Data.Table
{
    [TestClass()]
    public class TableTests
    {
        private string filePath = string.Empty;

        [TestMethod]
        public void DeleteFile()
        {
            var projectFileManager = new LocalProjectFilesManager();
            var filePath = Path.Combine(projectFileManager.FileDirInfo.FullName, "DeleteFile.text");

            File.WriteAllText(filePath, "Should delete me for test");

            Assert.IsTrue(string.IsNullOrEmpty(Task.Run(async () => await projectFileManager.DeleteAsync(filePath)).Result));

            Assert.IsFalse(File.Exists(filePath));
        }

        // Test depends on Identity in main code working. It is not currently working and is disabled in main code.
        //[TestMethod]
        //public void DeleteFail()
        //{
        //    var projectFileManager = new LocalProjectFilesManager();
        //    var filePath = Path.Combine(projectFileManager.FileDirInfo.FullName, "DeleteFileFail.text");

        //    IPrincipal principal = new GenericPrincipal(new GenericIdentity("test"), new string[] { "test" });

        //    File.WriteAllText(filePath, "Should delete me for test");

        //    Assert.IsFalse(string.IsNullOrEmpty(Task.Run(async () => await projectFileManager.DeleteAsync(filePath, principal)).Result));

        //    Assert.IsTrue(File.Exists(filePath));
        //}

        [TestMethod]
        public void Save()
        {
            var projectFileManager = new LocalProjectFilesManager();

            string testData = "TestData";
            string ext = ".txt";

            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                filePath  = Task.Run(async () => await projectFileManager.SaveAsync(ms, Guid.NewGuid().ToString(), ext)).Result;
            }

            Assert.IsFalse(string.IsNullOrEmpty(filePath));

            var fullPath = Path.Combine(projectFileManager.FileDirInfo.FullName, filePath);

            // Files are stored compressed on disk; the logical Location has no extension.
            Assert.IsTrue(File.Exists(fullPath + ".gz"), "Compressed file must be written to disk on save.");
            Assert.IsFalse(File.Exists(fullPath), "Uncompressed variant must not exist after save.");
        }

        [TestMethod]
        public void SaveReadRoundTripPreservesContent()
        {
            var projectFileManager = new LocalProjectFilesManager();

            const string testData = "Compressed roundtrip content - verify hydration on serve.";
            string ext = ".txt";

            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var location = Task.Run(async () => await projectFileManager.SaveAsync(ms, Guid.NewGuid().ToString(), ext)).Result;
                var fullPath = Path.Combine(projectFileManager.FileDirInfo.FullName, location);

                using var readStream = Task.Run(async () => await projectFileManager.ReadAsync(location)).Result;
                Assert.IsNotNull(readStream, "ReadAsync must return a stream for a saved file.");

                using var ms2 = new MemoryStream();
                readStream.CopyTo(ms2);
                var roundTrip = Encoding.UTF8.GetString(ms2.ToArray());

                Assert.AreEqual(testData, roundTrip, "Decompressed content must match the original.");
                Assert.IsTrue(File.Exists(fullPath + ".gz"), "Compressed file must persist on disk.");
            }
        }

        [TestMethod]
        public void ReadLegacyUncompressedFileStillWorks()
        {
            var projectFileManager = new LocalProjectFilesManager();

            const string testData = "Legacy uncompressed content.";
            var legacyPath = Path.Combine(projectFileManager.FileDirInfo.FullName, "LegacyFile.text");

            File.WriteAllText(legacyPath, testData);

            using var readStream = Task.Run(async () => await projectFileManager.ReadAsync("LegacyFile.text")).Result;
            Assert.IsNotNull(readStream, "ReadAsync must read legacy uncompressed files.");

            using var ms = new MemoryStream();
            readStream.CopyTo(ms);
            var readBack = Encoding.UTF8.GetString(ms.ToArray());

            Assert.AreEqual(testData, readBack, "Legacy file content must be readable without decompression.");
        }

        [TestMethod]
        public void DeleteRemovesCompressedAndLegacyVariants()
        {
            var projectFileManager = new LocalProjectFilesManager();

            const string testData = "Delete me in both formats.";
            string ext = ".txt";

            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var location = Task.Run(async () => await projectFileManager.SaveAsync(ms, Guid.NewGuid().ToString(), ext)).Result;
                var fullPath = Path.Combine(projectFileManager.FileDirInfo.FullName, location);

                Assert.IsTrue(File.Exists(fullPath + ".gz"), "Compressed file must exist before delete.");

                var result = Task.Run(async () => await projectFileManager.DeleteAsync(location, new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("test"), new[] { "test" }))).Result;

                Assert.IsNull(result, "Delete of an existing file must return null (no error).");
                Assert.IsFalse(File.Exists(fullPath + ".gz"), "Compressed file must be removed on delete.");
                Assert.IsFalse(File.Exists(fullPath), "Legacy variant must be removed on delete.");
            }
        }

        [TestMethod]
        public void Read()
        {
            var projectFileManager = new LocalProjectFilesManager();
            var filePath = Path.Combine(projectFileManager.FileDirInfo.FullName, "ReadTestFile.text");

            const string testDocument = "Hopefully this test can be read after passign through the read method";

            File.WriteAllText(filePath, testDocument);

            using(var readStream = Task.Run(async () => await projectFileManager.ReadAsync(filePath)).Result)
            {
                using (var ms = new MemoryStream())
                {
                    readStream.CopyTo(ms);

                    Assert.AreEqual(testDocument, Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (ProjectManagerService.BaseDirInfo.Exists)
            {
                foreach(var dirInfo in ProjectManagerService.BaseDirInfo.GetDirectories("*", SearchOption.AllDirectories))
                {
                    dirInfo.Delete(true);
                }
            }
        }

        [TestMethod]
        public void MigrateCompressesLegacyUncompressedFiles()
        {
            var projectFileManager = new LocalProjectFilesManager();

            const string testData = "Legacy content to migrate.";
            var legacyPath = Path.Combine(projectFileManager.FileDirInfo.FullName, "LegacyMigFile.text");
            File.WriteAllText(legacyPath, testData);

            Task.Run(async () => await projectFileManager.MigrateExistingFilesAsync()).Wait();

            // After migration, the .gz variant must exist and the raw file must be gone.
            Assert.IsTrue(File.Exists(legacyPath + ".gz"), "Migration must create a .gz compressed variant.");
            Assert.IsFalse(File.Exists(legacyPath), "Migration must remove the legacy uncompressed file.");

            // The migrated content must still be readable.
            using var readStream = Task.Run(async () => await projectFileManager.ReadAsync("LegacyMigFile.text")).Result;
            Assert.IsNotNull(readStream, "ReadAsync must read the migrated file.");

            using var ms = new MemoryStream();
            readStream.CopyTo(ms);
            var readBack = Encoding.UTF8.GetString(ms.ToArray());

            Assert.AreEqual(testData, readBack, "Migrated content must match the original.");
        }

        [TestMethod]
        public void MigrateIsIdempotentWhenAllFilesAreCompressed()
        {
            var projectFileManager = new LocalProjectFilesManager();

            const string testData = "Already compressed content.";
            string ext = ".txt";

            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                var location = Task.Run(async () => await projectFileManager.SaveAsync(ms, Guid.NewGuid().ToString(), ext)).Result;
                var fullPath = Path.Combine(projectFileManager.FileDirInfo.FullName, location);

                Assert.IsTrue(File.Exists(fullPath + ".gz"), "Saved file must be compressed.");

                // Running migration again on already-compressed files should be a safe no-op.
                Task.Run(async () => await projectFileManager.MigrateExistingFilesAsync()).Wait();

                Assert.IsTrue(File.Exists(fullPath + ".gz"), "Compressed file must still exist after re-migration.");
                Assert.IsFalse(File.Exists(fullPath), "No uncompressed variant should exist.");

                using var readStream = Task.Run(async () => await projectFileManager.ReadAsync(location)).Result;
                Assert.IsNotNull(readStream, "ReadAsync must still work after re-migration.");

                using var ms2 = new MemoryStream();
                readStream.CopyTo(ms2);
                Assert.AreEqual(testData, Encoding.UTF8.GetString(ms2.ToArray()), "Content must be unchanged after re-migration.");
            }
        }
    }
}