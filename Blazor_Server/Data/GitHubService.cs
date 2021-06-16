using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace Blazor_Server.Data
{
    public class GitHubService
    {
        private const string Org = "umgc";

        private GitHubClient client = new GitHubClient(new ProductHeaderValue("UMGC", "0.1"));

        public async Task<List<Repository>> GetRepositories()
        {
            // Get repositories for the org
            var repositories = await client.Repository.GetAllForOrg(Org);

            List<Repository> repos = new List<Repository>();

            // Sort by updated and skip any that are archived.
            foreach(var repo in repositories.OrderByDescending(r => r.UpdatedAt))
            {
                if (repo.Archived)
                {
                    continue;
                }

                repos.Add(repo);
            }

            return repos;
        }
    }
}
