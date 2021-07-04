using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace CaPPMS.Data
{
    public class GitHubService
    {
        public async Task<List<Repository>> GetRepositories(string org)
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue(org.ToUpper(), "0.1"));

            // Get repositories for the org
            var repositories = await client.Repository.GetAllForOrg(org.ToLower());

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
