using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CaPPMS.Model;
using Newtonsoft.Json;

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

        public async Task<bool> AddAsync(FaqInformation faqInformation)
        {
            if (FaqInfo.TryAdd(faqInformation.Guid, faqInformation))
            {
                FaqsChanged?.Invoke(FaqInfo, EventArgs.Empty);
                return await Task.FromResult(true);
            }

            return false;
        }

        private void FaqManagerService_FaqsChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
