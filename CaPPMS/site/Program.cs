using CaPPMS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaPPMS
{
    public class Program
    {
        public static IDictionary<object, object> HostProperties { get; private set; } = new Dictionary<object, object>();

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // One-time migration: compress legacy uncompressed files to .gz.
            // TODO: [willywil548] Remove this block once PPE and Live have both
            // been rolled out with compressed file storage and all existing
            // files have been migrated.
            var fileManager = new LocalProjectFilesManager();
            fileManager.MigrateExistingFilesAsync().GetAwaiter().GetResult();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            if (host.Properties != null)
            {
                HostProperties = host.Properties;
            }

            return host;
        }

        /// <summary>
        /// Get property
        /// </summary>
        /// <param name="key">Key to lookup.</param>
        /// <returns>Value.</returns>
        public static string GetConfigurationSetting(string key)
        {
            foreach (var item in Program.HostProperties)
            {
                if (item.Key is WebHostBuilderContext context)
                {
                    return context.Configuration[key] ?? string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
