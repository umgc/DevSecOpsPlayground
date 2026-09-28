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

            HasExpectedResults(validation, expectedErrorMessages);
        }

        [TestMethod]
        public void FileSizeUploadValidation()
        {
            var cut = RenderComponent<Idea>();

            var attachedTestFiles = GenerateAttachedFiles(11, 9, "testfile");

            cut.Instance.ProjectIdea.SetAttachments(attachedTestFiles);

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "Max file size (10) exceeded on: testfile,testfile,testfile,testfile,testfile,testfile,testfile,testfile,testfile."
            };

            HasExpectedResults(validation, expectedErrorMessages);

            string[] notExpectedErrorMessages = new[]
{
                "Exceeded max number of files. Max:50"
            };

            ExpectedNotToHave(validation, notExpectedErrorMessages);
        }

        [TestMethod]
        public void FileNumberUploadValidation()
        {
            var cut = RenderComponent<Idea>();

            var attachedTestFiles = GenerateAttachedFiles(9, 51, "testfile");

            cut.Instance.ProjectIdea.SetAttachments(attachedTestFiles);

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "Exceeded max number of files. Max:50."
            };

            HasExpectedResults(validation, expectedErrorMessages);

            string[] notExpectedErrorMessages = new[]
            {
                "Max file size (10) exceeded on: testfile."
            };

            ExpectedNotToHave(validation, notExpectedErrorMessages);
        }

        [TestMethod]
        public void PhoneFailsValidation()
        {
            var cut = RenderComponent<Idea>(parameters => parameters
            .Add(p => p.ProjectIdea, new ProjectInformation
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@email.com",
                Phone = "Some Text",
                ProjectDescription = "Test Project Description",
                ProjectTitle = "Test Project"
            }));

            IElement submitButton = cut.Find("#btnSubmit");

            submitButton.Click();

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "The Phone field is not a valid phone number."
            };

            HasExpectedResults(validation, expectedErrorMessages);

            string[] notExpectedErrorMessages = new[]
            {
                "The SponsorFirstName field is required.",
                "The SponsorLastName field is required.",
                "The SponsorEmail field is not a valid e-mail address."
            };

            ExpectedNotToHave(validation, notExpectedErrorMessages);
        }

        [TestMethod]
        public void FirstNameFieldDoesValidateLastNameDoesNot()
        {
            var cut = RenderComponent<Idea>(parameters => parameters
            .Add(p => p.ProjectIdea, new ProjectInformation
            {
                FirstName = new string('t', 49),
                LastName = new string('t', 51),
                Email = "Test@email.com",
                Phone = "555-555-5555",
                ProjectDescription = "Test Project Description",
                ProjectTitle = "Test Project"
            }));

            IElement submitButton = cut.Find("#btnSubmit");

            submitButton.Click();

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "Last name is too long."
            };

            HasExpectedResults(validation, expectedErrorMessages);

            string[] notExpectedErrorMessages = new[]
            {
                "First name is too long."
            };

            ExpectedNotToHave(validation, notExpectedErrorMessages);
        }

        [TestMethod]
        public void LastNameFieldDoesValidateFirstNameDoesNot()
        {
            var cut = RenderComponent<Idea>(parameters => parameters
            .Add(p => p.ProjectIdea, new ProjectInformation
            {
                FirstName = new string('t', 51),
                LastName = new string('t', 49),
                Email = "Test@email.com",
                Phone = "555-555-5555",
                ProjectDescription = "Test Project Description",
                ProjectTitle = "Test Project"
            }));

            IElement submitButton = cut.Find("#btnSubmit");

            submitButton.Click();

            IElement form = cut.Find("form");

            form.Submit();

            var validation = cut.Find("form > ul");

            string[] expectedErrorMessages = new[]
            {
                "First name is too long."
            };

            HasExpectedResults(validation, expectedErrorMessages);

            string[] notExpectedErrorMessages = new[]
            {
                "Last name is too long."
            };

            ExpectedNotToHave(validation, notExpectedErrorMessages);
        }

        #region Helper Methods

        private void HasExpectedResults(IElement elementToValidate, IEnumerable<string> expectedResluts)
        {
            foreach (var error in expectedResluts)
            {
                Assert.IsTrue(elementToValidate.Children.Any(c => c.InnerHtml.Equals(error)));
            }
        }

        private void ExpectedNotToHave(IElement elementToValidate, IEnumerable<string> notExpectedErrorMessages)
        {
            foreach (var error in notExpectedErrorMessages)
            {
                Assert.IsFalse(elementToValidate.Children.Any(c => c.InnerHtml.Equals(error)));
            }
        }

        private IList<IProjectFile> GenerateAttachedFiles(int fileSizeMb, int numberOfFiles, string fileName)
        {
            IList<IProjectFile> projectFiles= new List<IProjectFile>();

            for(int i=0; i<numberOfFiles; i++)
            {
                projectFiles.Add(new ProjectFile { Location = $"{fileName}----{fileName}", Size = fileSizeMb * 1024 * 1024 });
            }

            return projectFiles;
        }

        #endregion
    }
}
