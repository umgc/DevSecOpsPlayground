using CaPPMS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CaPPMSTests.Model
{
    [TestClass]
    public class StudentTests
    {
        [TestMethod]
        public void ParseData_ValidCsv_ReturnsStudents()
        {
            var csv = "LastName,FirstName,Email Address,Github.com account\nSmith,Jane,jane.smith@student.umgc.edu,janesmith\nDoe,John,john.doe@student.umgc.edu,johndoe";
            var students = Student.ParseData(csv, 1, out var dropped);
            Assert.AreEqual(2, students.Count);
            Assert.AreEqual(0, dropped.Count);
            Assert.AreEqual("Smith", students[0].LastName);
            Assert.AreEqual("Jane", students[0].FirstName);
            Assert.AreEqual("jane.smith@student.umgc.edu", students[0].Email);
            Assert.AreEqual("janesmith", students[0].GitHub);
            Assert.AreEqual(1, students[0].ClassId);
        }

        [TestMethod]
        public void ParseData_EmptyData_ThrowsArgumentException()
        {
            Assert.ThrowsException<System.ArgumentException>(() => Student.ParseData(string.Empty, 1, out var dropped));
        }

        [TestMethod]
        public void ParseData_NullData_ThrowsArgumentException()
        {
            Assert.ThrowsException<System.ArgumentException>(() => Student.ParseData(null!, 1, out var dropped));
        }

        [TestMethod]
        public void ParseData_InvalidClassId_ThrowsArgumentException()
        {
            Assert.ThrowsException<System.ArgumentException>(() => Student.ParseData("LastName,FirstName,Email Address,Github.com account", -1, out var dropped));
        }

        [TestMethod]
        public void ParseData_MissingEmail_DropsRecord()
        {
            var csv = "LastName,FirstName,Email Address,Github.com account\nSmith,Jane,,janesmith";
            var students = Student.ParseData(csv, 1, out var dropped);
            Assert.AreEqual(0, students.Count);
            Assert.AreEqual(1, dropped.Count);
        }

        [TestMethod]
        public void ParseData_InvalidEmail_DropsRecord()
        {
            var csv = "LastName,FirstName,Email Address,Github.com account\nSmith,Jane,not-an-email,janesmith";
            var students = Student.ParseData(csv, 1, out var dropped);
            Assert.AreEqual(0, students.Count);
            Assert.AreEqual(1, dropped.Count);
        }

        [TestMethod]
        public void ParseData_GithubWithAt_DropsRecord()
        {
            var csv = "LastName,FirstName,Email Address,Github.com account\nSmith,Jane,jane.smith@student.umgc.edu,user@domain";
            var students = Student.ParseData(csv, 1, out var dropped);
            Assert.AreEqual(0, students.Count);
            Assert.AreEqual(1, dropped.Count);
        }

        [TestMethod]
        public void SetTeam_UpdatesAssignedTeam()
        {
            var student = new Student { StudentId = 1, FirstName = "Jane", LastName = "Smith" };
            var team = new Team { TeamId = 5, Name = "Alpha", ClassId = 1 };
            student.SetTeam(team);
            Assert.AreEqual(5, student.TeamId);
            Assert.AreEqual("Alpha", student.TeamName);
        }

        [TestMethod]
        public void Clone_CreatesIndependentCopy()
        {
            var student = new Student { StudentId = 1, FirstName = "Jane", LastName = "Smith", ClassId = 1, TeamId = 2 };
            var clone = student.Clone();
            Assert.AreEqual(student.StudentId, clone.StudentId);
            Assert.AreEqual(student.FirstName, clone.FirstName);
            Assert.AreNotSame(student, clone);
            Assert.AreNotSame(student.AssignedTeam, clone.AssignedTeam);
        }

        [TestMethod]
        public void IsMatch_NullFilter_ReturnsTrue()
        {
            var student = new Student { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", GitHub = "janesmith" };
            Assert.IsTrue(student.IsMatch(null));
            Assert.IsTrue(student.IsMatch(string.Empty));
        }

        [TestMethod]
        public void IsMatch_MatchingFilter_ReturnsTrue()
        {
            var student = new Student { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", GitHub = "janesmith" };
            Assert.IsTrue(student.IsMatch("Jane"));
            Assert.IsTrue(student.IsMatch("Smith"));
            Assert.IsTrue(student.IsMatch("jane@test.com"));
            Assert.IsTrue(student.IsMatch("janesmith"));
        }

        [TestMethod]
        public void IsMatch_NonMatchingFilter_ReturnsFalse()
        {
            var student = new Student { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", GitHub = "janesmith" };
            Assert.IsFalse(student.IsMatch("Nobody"));
        }
    }
}
