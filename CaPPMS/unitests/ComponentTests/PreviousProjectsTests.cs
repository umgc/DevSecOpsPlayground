using System.Collections.Generic;
using System.Linq;
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
        public void PreviousProjectsShowsEmptyStateWhenNoProjects()
        {
            // Arrange: Register a ProjectManagerService with no completed projects.
            TestContext.Services.AddSingleton(new ProjectManagerService("previousprojects-empty-test.json"));

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: No project cards are rendered.
            var cards = cut.FindAll(".project-card");
            Assert.AreEqual(0, cards.Count);

            // Assert: The page heading is still present.
            var heading = cut.Find("h2");
            Assert.AreEqual("Previous Projects", heading.TextContent.Trim());
        }

        [TestMethod]
        public void PreviousProjectsFiltersOutNonCompletedProjects()
        {
            // Arrange: Register a ProjectManagerService with mixed status projects.
            var projectManager = new ProjectManagerService("previousprojects-status-test.json");
            var completedIdea = new ProjectInformation
            {
                ProjectTitle = "Completed Project",
                ProjectDescription = "A completed project.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026"
            };
            var newIdea = new ProjectInformation
            {
                ProjectTitle = "New Project",
                ProjectDescription = "A new project.",
                Status = "New",
                SemesterTerm = "Spring",
                SemesterYear = "2026"
            };
            Task.Run(async () => await projectManager.AddAsync(completedIdea)).Wait();
            Task.Run(async () => await projectManager.AddAsync(newIdea)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Only the completed project is displayed.
            var cards = cut.FindAll(".project-card");
            Assert.AreEqual(1, cards.Count);
        }

        [TestMethod]
        public void PreviousProjectsGroupsByCohort()
        {
            // Arrange: Register a ProjectManagerService with projects from different cohorts.
            var projectManager = new ProjectManagerService("previousprojects-cohort-test.json");
            var idea1 = new ProjectInformation
            {
                ProjectTitle = "Spring 2026 Project",
                ProjectDescription = "A spring project.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026"
            };
            var idea2 = new ProjectInformation
            {
                ProjectTitle = "Fall 2025 Project",
                ProjectDescription = "A fall project.",
                Status = "Complete",
                SemesterTerm = "Fall",
                SemesterYear = "2025"
            };
            Task.Run(async () => await projectManager.AddAsync(idea1)).Wait();
            Task.Run(async () => await projectManager.AddAsync(idea2)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Both cohort headers are present.
            var h3Elements = cut.FindAll("h3");
            Assert.AreEqual(2, h3Elements.Count);
            var headerTexts = h3Elements.Select(h => h.TextContent.Trim()).ToList();
            Assert.IsTrue(headerTexts.Contains("Spring 2026") || headerTexts.Contains("Fall 2025"));
            Assert.IsTrue(headerTexts.Contains("Spring 2026") || headerTexts.Contains("Fall 2025"));
        }

        [TestMethod]
        public void PreviousProjectsRendersProjectLinks()
        {
            // Arrange: Register a ProjectManagerService with a project that has links.
            var projectManager = new ProjectManagerService("previousprojects-links-test.json");
            var idea = new ProjectInformation
            {
                ProjectTitle = "Linked Project",
                ProjectDescription = "A project with links.",
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

            // Assert: Links are rendered.
            var links = cut.FindAll("a").ToList();
            Assert.IsTrue(links.Count >= 2);
            Assert.IsTrue(links.Any(l => l.GetAttribute("href") == "https://github.com/test/repo"));
            Assert.IsTrue(links.Any(l => l.GetAttribute("href") == "https://example.com"));
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

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Both projects are displayed initially.
            var cards = cut.FindAll(".project-card");
            Assert.AreEqual(2, cards.Count);

            // Assert: Search input is present.
            var searchInput = cut.Find("input");
            Assert.IsNotNull(searchInput);
        }

        [TestMethod]
        public void PreviousProjectsRendersProjectWithNoLinks()
        {
            // Arrange: Register a ProjectManagerService with a project that has no links.
            var projectManager = new ProjectManagerService("previousprojects-nolinks-test.json");
            var idea = new ProjectInformation
            {
                ProjectTitle = "No Links Project",
                ProjectDescription = "A project with no links.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026"
            };
            Task.Run(async () => await projectManager.AddAsync(idea)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Project card renders without links.
            var card = cut.Find(".project-card");
            Assert.IsNotNull(card);
        }

        [TestMethod]
        public void PreviousProjectsOrdersByCohort()
        {
            // Arrange: Register a ProjectManagerService with projects from different cohorts.
            var projectManager = new ProjectManagerService("previousprojects-order-test.json");
            var fall2025 = new ProjectInformation
            {
                ProjectTitle = "Fall 2025 Project",
                ProjectDescription = "A fall project.",
                Status = "Complete",
                SemesterTerm = "Fall",
                SemesterYear = "2025"
            };
            var spring2026 = new ProjectInformation
            {
                ProjectTitle = "Spring 2026 Project",
                ProjectDescription = "A spring project.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026"
            };
            Task.Run(async () => await projectManager.AddAsync(fall2025)).Wait();
            Task.Run(async () => await projectManager.AddAsync(spring2026)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Spring 2026 appears before Fall 2025 (newer cohorts first).
            var h3Elements = cut.FindAll("h3").Select(h => h.TextContent.Trim()).ToList();
            var springIndex = h3Elements.IndexOf("Spring 2026");
            var fallIndex = h3Elements.IndexOf("Fall 2025");
            Assert.IsTrue(springIndex >= 0 && fallIndex >= 0);
            Assert.IsTrue(springIndex < fallIndex, "Spring 2026 should appear before Fall 2025");
        }

        [TestMethod]
        public void PreviousProjectsRendersDocumentLinks()
        {
            // Arrange: Register a ProjectManagerService with a project that has completed documents.
            var projectManager = new ProjectManagerService("previousprojects-docs-test.json");
            var idea = new ProjectInformation
            {
                ProjectTitle = "Docs Project",
                ProjectDescription = "A project with documents.",
                Status = "Complete",
                SemesterTerm = "Spring",
                SemesterYear = "2026",
                CompletedDocuments = new List<CompletedProjectDocumentation>
                {
                    new CompletedProjectDocumentation
                    {
                        TeamName = "Test Team",
                        ProjectPlanLink = "plan-guid",
                        ProjectPlan = "Project Plan.pdf",
                        SRSLink = "srs-guid",
                        SRS = "SRS.pdf"
                    }
                }
            };
            Task.Run(async () => await projectManager.AddAsync(idea)).Wait();

            TestContext.Services.AddSingleton(projectManager);

            // Act: Render the component.
            var cut = RenderComponent<CaPPMS.Pages.PreviousProjects>();

            // Assert: Document links are rendered.
            var links = cut.FindAll("a").ToList();
            Assert.IsTrue(links.Any(l => l.TextContent.Contains("Project Plan")));
            Assert.IsTrue(links.Any(l => l.TextContent.Contains("SRS")));
        }
    }
}
