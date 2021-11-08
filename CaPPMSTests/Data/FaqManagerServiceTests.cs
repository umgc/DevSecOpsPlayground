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

            Assert.IsTrue(faqManagerService.Add(faq));
            Assert.AreEqual(1, faqManagerService.GetFaqs.Count());

            Assert.IsTrue(faq.Guid != Guid.Empty);
        }

        [TestMethod()]
        public void Remove()
        {
            FaqManagerService faqManagerService = new FaqManagerService("remove.json");
            var faq = CreateFaq();


            faqManagerService.Add(faq);
            Assert.AreEqual(1, faqManagerService.GetFaqs.Count());

          
            Assert.IsTrue(faqManagerService.Remove(faq));

            Assert.AreEqual(0, faqManagerService.GetFaqs.Count());
        }

        [TestMethod]
        public void fileDidSave()
        {
            FaqManagerService faqManagerService = new FaqManagerService("savefile.json");

            faqManagerService.Add(CreateFaq());


            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            var filePath = Path.Combine(FaqManagerService.BaseDirInfo.FullName, "savefile.json");

            Assert.IsTrue(File.Exists(filePath));

            var fileData = File.ReadAllText(filePath);

            var faqData = JsonConvert.DeserializeObject<Dictionary<string, FaqInformation>>(fileData);
            Assert.AreEqual(1, faqData.Count);
        }

        [TestMethod]
        public void didUpdateFaq()
        {
            string newQuestion = $"New question-{Guid.NewGuid()}";
            FaqManagerService faqManagerService = new FaqManagerService("update.json");
            var faq = CreateFaq();

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