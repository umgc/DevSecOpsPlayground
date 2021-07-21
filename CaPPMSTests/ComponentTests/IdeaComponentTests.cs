using AngleSharp.Dom;
using Bunit;
using CaPPMS.Model;
using CaPPMS.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CaPPMSTests.ComponentTests
{
    [TestClass]
    public class IdeaComponentTests : BunitTestHelper
    {
        [TestMethod]
        public void FailedBlankSubmission()
        {
            var cut = RenderComponent<Idea>();

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "The ProjectTitle field is required.",
                "The ProjectDescription field is required.",
                "The FirstName field is required.",
                "The LastName field is required.",
                "The Email field is not a valid e-mail address.",
                "The Phone field is not a valid phone number.",
                "The SponsorFirstName field is required.",
                "The SponsorLastName field is required.",
                "The SponsorEmail field is not a valid e-mail address.",
                "The SponsorPhone field is not a valid phone number."
            };

            foreach(var error in expectedErrorMessages)
            {
                Assert.IsTrue(validation.Children.Any(c => c.InnerHtml.Equals(error)));
            }
        }

        [TestMethod]
        public void FileSizeUploadValidation()
        {
            var cut = RenderComponent<Idea>();

            var attachedTestFiles = new List<IProjectFile>()
            {
                new ProjectFile{ Location = "testfile----testfile", Size = 11 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
            };

            cut.Instance.ProjectIdea.SetAttachments(attachedTestFiles);

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "Max file size (10) exceeded on: testfile."
            };

            foreach (var error in expectedErrorMessages)
            {
                Assert.IsTrue(validation.Children.Any(c => c.InnerHtml.Equals(error)));
            }

            string[] notExpectedErrorMessages = new[]
{
                "Exceeded max number of files. Max:10"
            };

            foreach (var error in notExpectedErrorMessages)
            {
                Assert.IsFalse(validation.Children.Any(c => c.InnerHtml.Equals(error)));
            }
        }

        [TestMethod]
        public void FileNumberUploadValidation()
        {
            var cut = RenderComponent<Idea>();

            var attachedTestFiles = new List<IProjectFile>()
            {
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
                new ProjectFile{ Location = "testfile----testfile", Size = 9 * 1024 * 1024},
            };

            cut.Instance.ProjectIdea.SetAttachments(attachedTestFiles);

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "Exceeded max number of files. Max:10."
            };

            foreach (var error in expectedErrorMessages)
            {
                Assert.IsTrue(validation.Children.Any(c => c.InnerHtml.Equals(error)));
            }

            string[] notExpectedErrorMessages = new[]
            {
                "Max file size (10) exceeded on: testfile."
            };

            foreach (var error in notExpectedErrorMessages)
            {
                Assert.IsFalse(validation.Children.Any(c => c.InnerHtml.Equals(error)));
            }
        }
    }
}
