using CaPPMS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace CaPPMSTests.Model
{
    [TestClass]
    public class ProjectInformationValidationTests
    {
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        [TestMethod]
        public void ProjectInformation_ValidModel_PassesValidation()
        {
            var project = new ProjectInformation
            {
                FirstName = "John",
                LastName = "Doe",
                ProjectTitle = "My Capstone Project",
                ProjectDescription = "A description of the project",
                Email = "john.doe@student.umgc.edu",
                Phone = "555-555-5555",
                SponsorFirstName = "Jane",
                SponsorLastName = "Sponsor",
                SponsorEmail = "sponsor@test.com",
                SponsorPhone = "555-555-5555"
            };

            var results = ValidateModel(project);
            Assert.AreEqual(0, results.Count, $"Expected no validation errors but got: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        [TestMethod]
        public void ProjectInformation_ShortProjectTitle_FailsValidation()
        {
            var project = new ProjectInformation
            {
                FirstName = "John",
                LastName = "Doe",
                ProjectTitle = "ABC",
                ProjectDescription = "A description",
                Email = "john.doe@student.umgc.edu"
            };

            var results = ValidateModel(project);
            Assert.IsTrue(results.Count > 0, "Short project title should fail validation.");
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("ProjectTitle")));
        }

        [TestMethod]
        public void ProjectInformation_EmptyFirstName_FailsValidation()
        {
            var project = new ProjectInformation
            {
                FirstName = string.Empty,
                LastName = "Doe",
                ProjectTitle = "My Capstone Project",
                ProjectDescription = "A description",
                Email = "john.doe@student.umgc.edu"
            };

            var results = ValidateModel(project);
            Assert.IsTrue(results.Count > 0, "Empty first name should fail validation.");
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("FirstName")));
        }

        [TestMethod]
        public void ProjectInformation_InvalidEmail_FailsValidation()
        {
            var project = new ProjectInformation
            {
                FirstName = "John",
                LastName = "Doe",
                ProjectTitle = "My Capstone Project",
                ProjectDescription = "A description",
                Email = "not-an-email"
            };

            var results = ValidateModel(project);
            Assert.IsTrue(results.Count > 0, "Invalid email should fail validation.");
        }

        [TestMethod]
        public void ProjectInformation_LongFirstName_FailsValidation()
        {
            var project = new ProjectInformation
            {
                FirstName = new string('A', 51),
                LastName = "Doe",
                ProjectTitle = "My Capstone Project",
                ProjectDescription = "A description",
                Email = "john.doe@student.umgc.edu"
            };

            var results = ValidateModel(project);
            Assert.IsTrue(results.Count > 0, "First name over 50 chars should fail validation.");
        }

        [TestMethod]
        public void Student_ValidModel_PassesValidation()
        {
            var student = new Student
            {
                StudentId = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@student.umgc.edu",
                GitHub = "janesmith",
                ClassId = 1,
                TeamId = 1,
                IsTeamLead = false
            };

            var results = ValidateModel(student);
            Assert.AreEqual(0, results.Count, $"Expected no validation errors but got: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        [TestMethod]
        public void Student_InvalidEmail_DoesNotFailValidation()
        {
            // Student.Email has no [EmailAddress] attribute, so invalid
            // email formats pass model validation.
            var student = new Student
            {
                StudentId = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "not-an-email",
                GitHub = "janesmith",
                ClassId = 1
            };

            var results = ValidateModel(student);
            Assert.AreEqual(0, results.Count, "Student.Email has no format validation attribute.");
        }

        [TestMethod]
        public void Contact_ValidModel_PassesValidation()
        {
            var contact = new Contact
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@student.umgc.edu",
                Phone = "555-555-5555"
            };

            var results = ValidateModel(contact);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Contact_DefaultValues_AreNotNull()
        {
            var contact = new Contact();
            Assert.AreNotEqual(Guid.Empty, contact.ContactID);
            Assert.IsNotNull(contact.FirstName);
            Assert.IsNotNull(contact.LastName);
            Assert.IsNotNull(contact.Email);
            Assert.IsNotNull(contact.Phone);
        }
    }
}

