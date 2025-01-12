using CaPPMS.Attributes;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CaPPMS.Model
{
    [SqlTableName("Students")]
    public class Student : ISqlTableModel
    {
        private const string EmailSuffix = "@student.umgc.edu";

        public Student() { }

        [Required]
        [SqlIdProperty]
        public long StudentId { get; set; } = default;

        [ColumnHeader]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [ColumnHeader]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        [ColumnHeader]
        public string Email { get; set; } = string.Empty;

        [ColumnHeader]
        public string GitHub { get; set; } = string.Empty;

        [ColumnHeader]
        [DisplayName("Team")]
        [IgnoreDataMember]
        public string? TeamName
        {
            get
            {
                return this.AssignedTeam.Name;
            }
            set
            {
                this.AssignedTeam.Name = value ?? string.Empty;
            }
        }

        [IgnoreDataMember]
        public Team AssignedTeam { get; private set; } = new Team();

        public long TeamId
        {
            get
            {
                return this.AssignedTeam.TeamId;
            }
            set
            {
                this.AssignedTeam.TeamId = value;
            }
        }

        public long ClassId { get; set; } = -1;

        /// <summary>
        /// Set the team for the student.
        /// </summary>
        /// <param name="team">Appropiate team.</param>
        public void SetTeam(Team team)
        {
            this.AssignedTeam = team;
        }

        public void SetProperty(string propertyName, object value)
        {
            var property = this.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                if (value is string stringData && string.IsNullOrEmpty(stringData))
                {
                    throw new InvalidOperationException("Value cannot be null or empty.");
                }

                property.SetValue(this, value);
            }
        }

        public Student Clone()
        {
            Student student = (Student)this.MemberwiseClone();
            Team team = new()
            {
                TeamId = this.AssignedTeam.TeamId,
                Name = this.AssignedTeam.Name,
                ClassId = this.AssignedTeam.ClassId
            };
            student.AssignedTeam = team;
            return student;
        }

        /// <summary>
        /// Parse the data from a CSV file.
        /// </summary>
        /// <param name="data">The CSV.</param>
        /// <returns><see cref="IEnumerable{Student}"/>.</returns>
        public static List<Student> ParseData(string data, long classId, out List<Student> droppedRecords)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("Data cannot be null or empty.");
            }

            if (classId < 0)
            {
                throw new ArgumentException("Invalid class ID.");
            }

            List<Student> students = new List<Student>();
            droppedRecords = new List<Student>();

            try
            {
                // Split the data into lines
                string[] lines = data
                    .Replace("\r", string.Empty)
                    .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string[] headerFields = lines[0].Split(',');
                if (headerFields.Length < 3)
                {
                    throw new Exception("Invalid file format.");
                }

                Dictionary<string, int> headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < headerFields.Length; i++)
                {
                    headerMap[headerFields[i].Trim()] = i;
                }

                // Iterate over each line (excluding the header)
                for (int i = 1; i < lines.Length; i++)
                {
                    string trimmedLine = lines[i].Trim();

                    // Skip empty lines.
                    if (string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        continue;
                    }

                    string[] fields = lines[i].Split(',');

                    // Create a new Student object
                    Student student = new Student();

                    // Populate the fields
                    student.LastName = fields[headerMap["LastName"]];
                    student.FirstName = fields[headerMap["FirstName"]];
                    student.Email = fields[headerMap["Email Address"]];
                    student.GitHub = fields[headerMap["Github.com account"]].Replace("https://github.com/", string.Empty);

                    // Validation
                    if (string.IsNullOrEmpty(student.LastName))
                    {
                        droppedRecords.Add(student);

                        // TODO: log
                        continue;
                    }

                    if (string.IsNullOrEmpty(student.FirstName))
                    {
                        droppedRecords.Add(student);

                        // TODO: log
                        continue;
                    }

                    if (!student.Email.EndsWith(EmailSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        droppedRecords.Add(student);

                        // TODO: log
                        continue;
                    }

                    if (student.GitHub.Contains("@"))
                    {
                        droppedRecords.Add(student);

                        // TODO: log
                        continue;
                    }

                    student.ClassId = classId;

                    // Add the student to the list
                    students.Add(student);
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                Console.WriteLine(ex.Message);
            }

            return students;
        }
    }
}