using System;
using System.Threading.Tasks;
using Octokit;
namespace CaPPMS.Data
{
    public class GitHubService
    {
        private GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("UMGCApp"));
        const string developmentBranch = "development";
        const string mainBranch = "main";
        const string heads = "heads/";
        

        public GitHubService()
        {
            gitHubClient.Credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
        }

        public async Task CreateRepository(string OrganizationName, string RepoName, string Description)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var repository = new NewRepository(RepoName)
                    {
                        AutoInit = true,
                        Description = Description,
                        Private = false
                    };
                    await gitHubClient.Repository.Create(OrganizationName, repository);
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
                    var masterReference = await gitHubClient.Git.Reference.Get(OrganizationName, RepoName, heads + mainBranch);
                    var branchReference = new NewReference(heads + developmentBranch, masterReference.Object.Sha);
                    await gitHubClient.Git.Reference.Create(OrganizationName, RepoName, branchReference);
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
                    var repoUpdateVar = new RepositoryUpdate(RepoName) { DefaultBranch = developmentBranch };
                    await gitHubClient.Repository.Edit(OrganizationName, RepoName, repoUpdateVar);
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
                    await gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, mainBranch, protection);
                    await gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, developmentBranch, protection);
                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: Unable to set branch protection - {e.Message}");
                }
            }).ConfigureAwait(false);
        }

        public async Task DoAllTasks(string OrganizationName, string RepoName, string Description)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var repository = new NewRepository(RepoName)
                    {
                        AutoInit = true,
                        Description = Description,
                        Private = false
                    };
                    var newRepository = gitHubClient.Repository.Create(OrganizationName, repository).GetAwaiter().GetResult();
                    await Task.Delay(2000);

                    var masterReference = await gitHubClient.Git.Reference.Get(OrganizationName, RepoName, heads + mainBranch);
                    var branchReference = new NewReference(heads + developmentBranch, masterReference.Object.Sha);
                    _ = gitHubClient.Git.Reference.Create(OrganizationName, RepoName, branchReference);

                    var repoUpdateVar = new RepositoryUpdate(RepoName) { DefaultBranch = developmentBranch };
                    _ = gitHubClient.Repository.Edit(OrganizationName, RepoName, repoUpdateVar);

                    var protection = new BranchProtectionSettingsUpdate(
                        new BranchProtectionRequiredReviewsUpdate(false, true, 1)
                    );
                    _ = gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, mainBranch, protection);
                    _ = gitHubClient.Repository.Branch.UpdateBranchProtection(OrganizationName, RepoName, developmentBranch, protection);

                }
                catch (AggregateException e)
                {
                    Console.Error.WriteLine($"E: For some reason, the repository {RepoName}  can't be created. It may already exist. {e.Message}");
                }
            }).ConfigureAwait(false);
        }
    }
}