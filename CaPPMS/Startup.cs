using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web.UI;
using CaPPMS.Data;

using System;

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
            Configuration["AzureAd:ClientSecret"] = GetClientSecret();
            string[]? initialScopes = Configuration.GetValue<string>("Graph:Scopes")?
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (initialScopes == null)
            {
                initialScopes = new[]
                {
                    "user.read"
                };
            }

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddMicrosoftGraph(Configuration.GetSection("Graph"))
                .AddInMemoryTokenCaches();

            services.AddHttpContextAccessor();

            services.AddRazorPages()
                  .AddMicrosoftIdentityUI();
                
            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

            services.AddSingleton<ProjectManagerService>();
            services.AddSingleton<FaqManagerService>();
            services.AddSingleton<GitHubService>();
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

        private string GetClientSecret()
        {
            string? secret = System.Environment.GetEnvironmentVariable("GRAPH_SECRET");

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException($"Graph Secret could not be retrieved from the environment variables.");
            }

            return secret;
        }
    }
}
