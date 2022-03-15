using System;
using System.Threading.Tasks;
using Octokit;
namespace CaPPMS.Data
{
    public class GitHubService
    {
        private GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("UMGCApp"));

        public GitHubService()
        {
            gitHubClient.Credentials = new Credentials(System.Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
        }

        public async Task CreateRepository(string OrganizationName, string RepoName, string Description)
        {
            await Task.Run(() =>
            {
                // Create 
                try
                {
                    var repository = new NewRepository(RepoName)
                    {
                        AutoInit = true,
                        Description = Description,
                        Private = false
                    };
                    var newRepository = System.Threading.Tasks.Task.Run(async () => await gitHubClient.Repository.Create(OrganizationName, repository).ConfigureAwait(false)); ;
                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: For some reason, the repository {RepoName}  can't be created. It may already exist. {e.Message}");
                }
            }).ConfigureAwait(false);
        }

        public async Task CreateDevBranch(string OrganizationName, string RepoName)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var masterReference = await gitHubClient.Git.Reference.Get(OrganizationName, RepoName, "heads/main").ConfigureAwait(false);
                    var branchReference = new NewReference("heads/" + "development", masterReference.Object.Sha);
                    await gitHubClient.Git.Reference.Create(OrganizationName, RepoName, branchReference).ConfigureAwait(false);
                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: For some reason, the developement branch can't be created - {e.Message}");
                }
            }).ConfigureAwait(false);
        }
        public async Task UpdateDefaultBranch(string OrganizationName, string RepoName)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var repoUpdateVar = new RepositoryUpdate(RepoName) { DefaultBranch = "development" };
                    await gitHubClient.Repository.Edit(OrganizationName, RepoName, repoUpdateVar).ConfigureAwait(false);
                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: The development branch can not be set as the default branch - {e.Message}");
                }
            }).ConfigureAwait(false);
        }

        public async Task AddBranchProtection(string OrganizationName, string RepoName)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var protection = new BranchProtectionSettingsUpdate(
                        new BranchProtectionRequiredReviewsUpdate(false, true, 1)
                    );
                    await gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, "main", protection).ConfigureAwait(false);
                    await gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, "development", protection).ConfigureAwait(false);
                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: Unable to set branch protection - {e.Message}");
                }
            }).ConfigureAwait(false);
        }

        public async Task doAllTasks(string OrganizationName, string RepoName, string Description)
        {
            await Task.Run(async () =>
            { 
                // Create 
                try
                {
                    var repository = new NewRepository(RepoName)
                    {
                        AutoInit = true,
                        Description = Description,
                        Private = false
                    };
                    var newRepository = gitHubClient.Repository.Create(OrganizationName, repository).GetAwaiter().GetResult();
                    await Task.Delay(2000).ConfigureAwait(false);

                    var masterReference = await gitHubClient.Git.Reference.Get(OrganizationName, RepoName, "heads/main");
                    var branchReference = new NewReference("heads/" + "development", masterReference.Object.Sha);
                    _ = gitHubClient.Git.Reference.Create(OrganizationName, RepoName, branchReference).GetAwaiter().GetResult();

                    var repoUpdateVar = new RepositoryUpdate(RepoName) { DefaultBranch = "development" };
                    _ = gitHubClient.Repository.Edit(OrganizationName, RepoName, repoUpdateVar).GetAwaiter().GetResult();

                    var protection = new BranchProtectionSettingsUpdate(
                        new BranchProtectionRequiredReviewsUpdate(false, true, 1)
                    );
                    _ = gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, "main", protection).GetAwaiter().GetResult();
                    _ = gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, "development", protection).GetAwaiter().GetResult();


                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: For some reason, the repository {RepoName}  can't be created. It may already exist. {e.Message}");
                }
            }).ConfigureAwait(false);

        }
    }
}