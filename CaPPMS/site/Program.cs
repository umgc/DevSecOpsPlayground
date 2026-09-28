using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace CaPPMS
{
    public class Program
    {
        public static IDictionary<object, object> HostProperties { get; private set; } = new Dictionary<object, object>();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
