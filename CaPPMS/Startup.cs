using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web.UI;
using CaPPMS.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using Azure.Security.KeyVault.Secrets;
using System;
using Azure.Identity;
using Azure;

namespace CaPPMS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string[] initialScopes = Configuration.GetValue<string>("Graph:Scopes")?.Split(' ');

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddMicrosoftGraph(Configuration.GetSection("Graph"))
                .AddInMemoryTokenCaches();

            string tenantId = Configuration.GetValue<string>("AzureAd:TenantId");
            services.Configure<MicrosoftIdentityOptions>(
               options => { options.ClientSecret = GetClientSecret(); });

            services.AddHttpContextAccessor();

            services.AddRazorPages()
                  .AddMicrosoftIdentityUI();
                
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

            services.AddSingleton<ProjectManagerService>();
            services.AddSingleton<FaqManagerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        /// Gets the secret from key vault via an enabled Managed Identity.
        /// </summary>
        /// <remarks>https://github.com/Azure-Samples/app-service-msi-keyvault-dotnet/blob/master/README.md</remarks>
        /// <returns></returns>
        private string GetClientSecret()
        {
#if (DEBUG)
            // Programmer issued secret. Limited time.
            return string.Empty;
#else
            return System.Environment.GetEnvironmentVariable("GRAPH_SECRET");
#endif


        }
    }
}
