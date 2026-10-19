﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaPPMS.Data;
using CaPPMS.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaPPMSTests.Data
{
    [TestClass()]
    public class DBOperationsServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var testDir = AppContext.BaseDirectory;
            foreach (var file in Directory.GetFiles(testDir, "test-*.db"))
            {
                try { File.Delete(file); } catch (IOException) { }
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            var testDir = AppContext.BaseDirectory;
            foreach (var file in Directory.GetFiles(testDir, "test-*.db"))
            {
                try { File.Delete(file); } catch (IOException) { }
            }
        }

        private string GetUniqueDbName(string testName)
        {
            return $"test-{testName}-{Guid.NewGuid():N}.db";
        }

        #region Constructor tests

        [TestMethod()]
        public void Constructor_WithNullLogger_DoesNotThrow()
        {
            var service = new DBOperationsService(GetUniqueDbName("ctor-null-logger"), null);
            Assert.IsNotNull(service);
        }

        [TestMethod()]
        public void Constructor_WithEmptyPath_UsesDefaultPath()
        {
            var service = new DBOperationsService("", null);
            Assert.IsNotNull(service);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithPathOutsideBaseDirectory_ThrowsArgumentException()
        {
            var service = new DBOperationsService("../../../escape.db", null);
        }

        #endregion

        #region Read query tests (no database)

        [TestMethod()]
        public async Task GetStudentsByClassAsync_WithNoDatabase_ReturnsEmptyList()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-students-by-class"), null);
            var result = await service.GetStudentsByClassAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async Task GetStudentsByTeamAsync_WithNoDatabase_ReturnsEmptyList()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-students-by-team"), null);
            var result = await service.GetStudentsByTeamAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async Task GetStudentScoresAsync_WithNoDatabase_ReturnsEmptyList()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-student-scores"), null);
            var result = await service.GetStudentScoresAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async Task GetStudentScoresAsync_WithStudentId_WithNoDatabase_ReturnsEmptyList()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-student-scores-by-id"), null);
            var result = await service.GetStudentScoresAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async Task GetStudentScoreDetails_WithNoDatabase_ReturnsEmptyList()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-student-score-details"), null);
            var result = await service.GetStudentScoreDetails();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod()]
        public async Task GetRecords_WithNoDatabase_ReturnsEmptyList()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-records"), null);
            var result = await service.GetRecords<Student>(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region Weeks tests

        [TestMethod()]
        public void GetNumberOfWeeks_ReturnsDefaultWhenNotConfigured()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-number-of-weeks"), null);
            var result = service.GetNumberOfWeeks();
            Assert.AreEqual(12, result);
        }

        [TestMethod()]
        public void GetWeeks_ReturnsCorrectNumberOfWeeks()
        {
            var service = new DBOperationsService(GetUniqueDbName("get-weeks"), null);
            var result = service.GetWeeks();
            Assert.IsNotNull(result);
            Assert.AreEqual(12, result.Count());
            Assert.AreEqual(1, result[0].Item1);
            Assert.AreEqual("One", result[0].Item2);
        }

        #endregion

        #region Add/Update/Remove record tests

        [TestMethod()]
        public async Task AddRecord_WithNullRecord_ThrowsArgumentNullException()
        {
            var service = new DBOperationsService(GetUniqueDbName("add-null-record"), null);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.AddRecord<Student>(null));
        }

        [TestMethod()]
        public async Task UpdateRecord_WithNullRecord_ThrowsArgumentNullException()
        {
            var service = new DBOperationsService(GetUniqueDbName("update-null-record"), null);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.UpdateRecord<Student>(null));
        }

        [TestMethod()]
        public async Task RemoveRecord_WithNullRecord_ThrowsArgumentNullException()
        {
            var service = new DBOperationsService(GetUniqueDbName("remove-null-record"), null);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.RemoveRecord<Student>(null));
        }

        [TestMethod()]
        public async Task AddRecord_WithValidStudent_ReturnsTrue()
        {
            var service = new DBOperationsService(GetUniqueDbName("add-valid-student"), NullLogger<DBOperationsService>.Instance);
            await service.EnsureDbExistsAsync();

            var student = new Student
            {
                StudentId = 1,
                FirstName = "Test",
                LastName = "Student",
                Email = "test@student.umgc.edu",
                GitHub = "testuser",
                ClassId = 1,
                TeamId = 1,
                IsTeamLead = false
            };

            var result = await service.AddRecord(student);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task AddRecord_ThenGetRecords_ReturnsAddedStudent()
        {
            var service = new DBOperationsService(GetUniqueDbName("add-then-get"), NullLogger<DBOperationsService>.Instance);
            await service.EnsureDbExistsAsync();

            var student = new Student
            {
                FirstName = "Added",
                LastName = "Student",
                Email = "added@student.umgc.edu",
                GitHub = "addeduser",
                ClassId = 7,
                TeamId = 3,
                IsTeamLead = true
            };
            await service.AddRecord(student);

            // StudentId is auto-incremented by the DB, so query all and find ours
            var all = await service.GetRecords<Student>();
            var added = all.FirstOrDefault(s => s.FirstName == "Added" && s.LastName == "Student");
            Assert.IsNotNull(added, "Added student should be found in the database.");
            Assert.AreEqual("Added", added.FirstName);
            Assert.AreEqual("Student", added.LastName);
            Assert.AreEqual("added@student.umgc.edu", added.Email);
        }

        [TestMethod()]
        public async Task UpdateRecord_WithValidStudent_ReturnsTrue()
        {
            var service = new DBOperationsService(GetUniqueDbName("update-valid-student"), NullLogger<DBOperationsService>.Instance);
            await service.EnsureDbExistsAsync();

            var student = new Student
            {
                FirstName = "Original",
                LastName = "Student",
                Email = "original@student.umgc.edu",
                GitHub = "origuser",
                ClassId = 5,
                TeamId = 2,
                IsTeamLead = false
            };
            await service.AddRecord(student);

            // Find the actual auto-assigned ID
            var all = await service.GetRecords<Student>();
            var added = all.FirstOrDefault(s => s.FirstName == "Original" && s.LastName == "Student");
            Assert.IsNotNull(added);

            // Update using the actual ID
            added.FirstName = "Updated";
            added.LastName = "Name";
            var result = await service.UpdateRecord(added);
            Assert.IsTrue(result);

            var fetched = await service.GetRecords<Student>(added.StudentId);
            var students = fetched.ToList();
            Assert.AreEqual(1, students.Count);
            Assert.AreEqual("Updated", students[0].FirstName);
            Assert.AreEqual("Name", students[0].LastName);
        }

        [TestMethod()]
        public async Task RemoveRecord_WithValidStudent_ReturnsTrue()
        {
            var service = new DBOperationsService(GetUniqueDbName("remove-valid-student"), NullLogger<DBOperationsService>.Instance);
            await service.EnsureDbExistsAsync();

            var student = new Student
            {
                FirstName = "ToRemove",
                LastName = "Student",
                Email = "remove@student.umgc.edu",
                GitHub = "removeuser",
                ClassId = 3,
                TeamId = 1,
                IsTeamLead = false
            };
            await service.AddRecord(student);

            // Find the actual auto-assigned ID
            var all = await service.GetRecords<Student>();
            var toRemove = all.FirstOrDefault(s => s.FirstName == "ToRemove" && s.LastName == "Student");
            Assert.IsNotNull(toRemove);

            var result = await service.RemoveRecord(toRemove);
            Assert.IsTrue(result);

            var remaining = await service.GetRecords<Student>(toRemove.StudentId);
            Assert.AreEqual(0, remaining.Count());
        }

        #endregion

        #region EnsureDbExistsAsync tests

        [TestMethod()]
        public async Task EnsureDbExistsAsync_CreatesDatabaseFile()
        {
            var dbName = GetUniqueDbName("ensure-db-exists");
            var service = new DBOperationsService(dbName, NullLogger<DBOperationsService>.Instance);

            await service.EnsureDbExistsAsync();

            var testDir = AppContext.BaseDirectory;
            var dbPath = Path.Combine(testDir, dbName);
            Assert.IsTrue(File.Exists(dbPath), "Database file should be created by EnsureDbExistsAsync.");
        }

        #endregion
    }
}
