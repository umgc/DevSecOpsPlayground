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
    public class FaqManagerServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestMethod()]
        public void Add()
        {
            FaqManagerService faqManagerService = new FaqManagerService("add.json");
            var faq = CreateFaq();
            //Assert.IsTrue(Task.Run(async () => await faqManagerService.AddAsync(idea)).Result);
            Assert.IsTrue(faqManagerService.Add(faq));
            Assert.AreEqual(1, faqManagerService.GetFaqs.Count());

            Assert.IsTrue(faq.Guid != Guid.Empty);
        }

        [TestMethod()]
        public void Remove()
        {
            FaqManagerService faqManagerService = new FaqManagerService("remove.json");
            var faq = CreateFaq();

            //Task.Run(async () => await faqManagerService.AddAsync(faq)).Wait();
            faqManagerService.Add(faq);
            Assert.AreEqual(1, faqManagerService.GetFaqs.Count());

            //Assert.IsTrue(Task.Run(async () => await faqManagerService.RemoveAsync(faq)).Result);
            Assert.IsTrue(faqManagerService.Remove(faq));

            //// Need a delay for Async testing
            //Task.Delay(TimeSpan.FromSeconds(1)).Wait();

            Assert.AreEqual(0, faqManagerService.GetFaqs.Count());
        }

        [TestMethod]
        public void FileDidSave()
        {
            FaqManagerService faqManagerService = new FaqManagerService("savefile.json");
            //Task.Run(async () => await faqManagerService.AddAsync(CreateFaq())).Wait();
            faqManagerService.Add(CreateFaq());

            // Wait on second to over come flaky test syndrome.
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            var filePath = Path.Combine(FaqManagerService.BaseDirInfo.FullName, "savefile.json");

            Assert.IsTrue(File.Exists(filePath));

            var fileData = File.ReadAllText(filePath);

            var faqData = JsonConvert.DeserializeObject<Dictionary<string, FaqInformation>>(fileData);
            Assert.AreEqual(1, faqData.Count);
        }

        [TestMethod]
        public void DidUpdateProjectManager()
        {
            string newQuestion = $"New question-{Guid.NewGuid()}";
            FaqManagerService faqManagerService = new FaqManagerService("update.json");
            var faq = CreateFaq();
            //Assert.IsTrue(Task.Run(async () => await faqManagerService.AddAsync(faq)).Result);
            Assert.IsTrue(faqManagerService.Add(faq));
            Assert.AreEqual(1, faqManagerService.GetFaqs.Count());

            faq.Question = newQuestion;

            Assert.IsTrue(Task.Run(async () => await faqManagerService.Update(faq)).Result);

            Assert.AreEqual(newQuestion, faqManagerService.GetFaqs.ToList()[0].Question);
        }

        private FaqInformation CreateFaq()
        {
            var idea = new FaqInformation
            {
                Author = "Author",
                Question = "Question?",
                Topic = "Topic",
                Reply = "Reply"
            };

            return idea;
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete faq file.
            foreach (var file in FaqManagerService.BaseDirInfo.GetFiles())
            {
                file.Delete();
            }
        }
    }
}