using System.Threading.Tasks;
using Bunit;
using CaPPMSTests.ComponentTests;
using CaPPMS.Data;
using CaPPMS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.ComponentTests
{
    [TestClass]
    public class PreviousProjectsTests : BunitTestHelper
    {
        [TestMethod]
        public void PreviousProjectsRendersWithoutCrashing()
        {
            // Arrange: Register a ProjectManagerService with no projects.
            TestContext.Services.AddSingleton(new ProjectManagerService("previousprojects-test.json"));

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Component renders with the page heading.
            var heading = cut.Find("h2");
            Assert.IsNotNull(heading);
            Assert.AreEqual("Previous Projects", heading.TextContent.Trim());
        }

        [TestMethod]
        public void PreviousProjectsDisplaysProjectCards()
        {
            // Arrange: Register a ProjectManagerService with a completed project.
            var projectManager = new ProjectManagerService("previousprojects-cards-test.json");
            var idea = new ProjectInformation
            {
                ProjectTitle = "Test Project",
                ProjectDescription = "A test project description.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026",
                Github = "https://github.com/test/repo",
                Url = "https://example.com"
            };
            Task.Run(async () => await projectManager.AddAsync(idea)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Project card is rendered.
            var card = cut.Find(".project-card");
            Assert.IsNotNull(card);

            var title = cut.Find("span").TextContent.Trim();
            Assert.AreEqual("Test Project", title);

            // Find the description paragraph within the card (not the page intro)
            var description = cut.Find(".project-card p").TextContent.Trim();
            Assert.AreEqual("A test project description.", description);
        }

        [TestMethod]
        public void PreviousProjectsSearchFiltersResults()
        {
            // Arrange: Register a ProjectManagerService with multiple projects.
            var projectManager = new ProjectManagerService("previousprojects-search-test.json");
            var idea1 = new ProjectInformation
            {
                ProjectTitle = "Alpha Project",
                ProjectDescription = "First test project.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026"
            };
            var idea2 = new ProjectInformation
            {
                ProjectTitle = "Beta Project",
                ProjectDescription = "Second test project.",
                Status = "Complete",
                SemesterTerm = "Fall",
                SemesterYear = "2025"
            };
            Task.Run(async () => await projectManager.AddAsync(idea1)).Wait();
            Task.Run(async () => await projectManager.AddAsync(idea2)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component and search for "Alpha".
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Initially both projects should be displayed.
            var cardsBefore = cut.FindAll(".project-card");
            Assert.AreEqual(2, cardsBefore.Count);

            // Find the search input and type "Alpha".
            var searchInput = cut.Find("input");
            searchInput.Change("Alpha");

            // Re-render to apply the filter.
            cut.Render();

            // Only the Alpha project should remain.
            var cardsAfter = cut.FindAll(".project-card");
            Assert.AreEqual(1, cardsAfter.Count);
        }
    }
}
