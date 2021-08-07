using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CaPPMS.Model;
using Newtonsoft.Json;
using System;

namespace CaPPMS.Data
{
    public class FaqManagerService
    {
        public static event EventHandler FaqsChanged;

        private readonly string localFaqDb = Path.Combine(BaseDirInfo.FullName, "faqs.json");

        public static DirectoryInfo BaseDirInfo { get; } = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredData"));

        public FaqManagerService()
        {
            if (!BaseDirInfo.Exists)
            {
                BaseDirInfo.Create();
            }
            var faqDbFile = new FileInfo(localFaqDb);

            if (faqDbFile.Exists)
            {
                var faqs = JsonConvert.DeserializeObject<Dictionary<Guid, FaqInformation>>(File.ReadAllText(faqDbFile.FullName));

                foreach (var faq in faqs)

                {
                    _ = FaqInfo.TryAdd(faq.Key, faq.Value);

                }

                FaqsChanged += FaqManagerService_FaqsChanged;
            }
        }
        public ConcurrentDictionary<Guid, FaqInformation> FaqInfo { get; } = new();

        public bool Add(FaqInformation faqInformation)
        {
            if (FaqInfo.TryAdd(faqInformation.Guid, faqInformation))
            {
                FaqsChanged?.Invoke(FaqInfo, EventArgs.Empty);

                return true;
            }

            return false;
        }

        public bool Remove(FaqInformation faqInformation)
        {
            if (!FaqInfo.TryRemove(faqInformation.Guid, out FaqInformation faq))
            {
                return false;
            }
            FaqsChanged?.Invoke(FaqInfo, EventArgs.Empty);

            return true;
        }

        public ICollection<FaqInformation> GetFaqs => FaqInfo.Values;

        private void FaqManagerService_FaqsChanged(object sender, EventArgs e)
        {
            var tempFile = new FileInfo(Path.Combine(localFaqDb + ".temp"));

            // Let's build a gate to control flow. It might be a bit extra but it should be fun.
            _ = Task.Run(async () =>

            {
                while (File.Exists(tempFile.FullName))
                {
                    // Use a prime number to reduce the chance of a race condition.
                    await Task.Delay(7);
                }
            }
            ).ContinueWith((context) =>
            {
                // Update the file backed db.
                lock (FaqInfo)
                {
                    // The task to move some time happens before the os knows that it exists =0
                    _ = Task.Run(async () =>
                    {
                        File.WriteAllText(tempFile.FullName, JsonConvert.SerializeObject(FaqInfo, Formatting.Indented));

                        while (!File.Exists(tempFile.FullName))
                        {
                            await Task.Delay(10);
                        }

                        tempFile.MoveTo(localFaqDb, true);
                    });
                }
            });
        }

        public Task<bool> Update(FaqInformation faqInformation)
        {
            bool completed;

            if (completed = FaqInfo.TryUpdate(faqInformation.Guid, faqInformation, FaqInfo[faqInformation.Guid]))
            {
                FaqsChanged?.Invoke(FaqInfo.Values, EventArgs.Empty);
            }

            return Task.FromResult(completed);
        }

        public IEnumerable<FaqInformation> GetFaqInfos()
        {
            List<FaqInformation> FaqInformations = new List<FaqInformation>();

            foreach (FaqInformation faq in FaqInfo.Values)

            {
                FaqInformations.Add(faq);
            }

            return new List<FaqInformation>();
        }
    }
}