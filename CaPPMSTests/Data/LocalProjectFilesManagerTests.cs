using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaPPMS.Data;

namespace CaPPMSTests.Data.Table
{
    [TestClass()]
    public class TableTests
    {
        private string filePath = string.Empty;

        [TestMethod]
        public void deleteFile()
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