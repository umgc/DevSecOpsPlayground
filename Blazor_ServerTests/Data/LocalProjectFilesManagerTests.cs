using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blazor_Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Principal;

namespace Blazor_Server.Data.Tests
{
    [TestClass()]
    public class LocalProjectFilesManagerTests
    {
        private string filePath = string.Empty;

        [TestMethod]
        public void DeleteFile()
        {
            var projectFileManager = new LocalProjectFilesManager();
            var filePath = Path.Combine(projectFileManager.FileDirInfo.FullName, "DeleteFile.text");

            File.WriteAllText(filePath, "Should delete me for test");

            Assert.IsTrue(Task.Run(async () => await projectFileManager.DeleteAsync(filePath)).Result);

            Assert.IsFalse(File.Exists(filePath));
        }

        [TestMethod]
        public void DeleteFail()
        {
            var projectFileManager = new LocalProjectFilesManager();
            var filePath = Path.Combine(projectFileManager.FileDirInfo.FullName, "DeleteFileFail.text");

            IPrincipal principal = new GenericPrincipal(new GenericIdentity("test"), new string[] { "test" });

            File.WriteAllText(filePath, "Should delete me for test");

            Assert.IsFalse(Task.Run(async () => await projectFileManager.DeleteAsync(filePath, principal)).Result);

            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void Save()
        {
            var projectFileManager = new LocalProjectFilesManager();

            string testData = "TestData";
            string ext = ".txt";

            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(testData)))
            {
                filePath  = Task.Run(async () => await projectFileManager.SaveAsync(ms, ext)).Result;
            }

            Assert.IsFalse(string.IsNullOrEmpty(filePath));

            filePath = Path.Combine(projectFileManager.FileDirInfo.FullName, filePath);

            Assert.IsTrue(File.Exists(filePath));
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
    }
}