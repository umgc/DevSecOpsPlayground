using CaPPMS.Attributes;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CaPPMS.Extensions;
using System.Linq;

namespace CaPPMS.Model
{
    /// <summary>
    /// Model for the Student.
    /// </summary>
    [SqlTableName("Students")]
    public class Student : ISqlTableModel
    {
        private const string EmailSuffix = "@student.umgc.edu";

        /// <summary>
        /// Initialize a new instance of the <see cref="Student"/> class.
        /// </summary>
        public Student() { }

        /// <summary>
        /// Gets or sets the student ID.
        /// </summary>
        [Required]
        [SqlIdProperty]
        public long StudentId { get; set; } = -1;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [ColumnHeader]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [ColumnHeader]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [ColumnHeader]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the GitHub account.
        /// </summary>
        [ColumnHeader]
        public string GitHub { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the team name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the team assigned to the student.
        /// </summary>
        [IgnoreDataMember]
        public Team AssignedTeam { get; private set; } = new Team();

        /// <summary>
        /// Gets or sets the team ID.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the class ID.
        /// </summary>
        public long ClassId { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating whether the student is a team leader.
        /// </summary>
        [ColumnHeader]
        public bool IsTeamLead { get; set; } = false;

        /// <summary>
        /// Set the team for the student.
        /// </summary>
        /// <param name="team">Appropiate team.</param>
        public void SetTeam(Team team)
        {
            this.AssignedTeam = team.Clone();
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
        public static List<Student> ParseData(string data, long classId, out List<Tuple<Student, string>> droppedRecords)
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
            droppedRecords = [];

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

                    string error = string.Empty;

                    // Validation
                    if (string.IsNullOrEmpty(student.LastName))
                    {
                        error += "Last name is required.";
                    }

                    if (string.IsNullOrEmpty(student.FirstName))
                    {
                        error += ", First name is required.";
                    }

                    if (!student.Email.EndsWith(EmailSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        error += $", Invalid email address. Given address={student.Email}";
                    }

                    if (student.GitHub.Contains("@"))
                    {
                        error += $", Invalid GitHub username. Given Github={student.GitHub}";
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        error = error.TrimStart(',', ' ');
                        droppedRecords.Add(Tuple.Create(student, error));
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

        /// <summary>
        /// Check if the object matches the filter.
        /// </summary>
        /// <param name="filter">Search Term.</param>
        /// <returns>If object matches the filter.</returns>
        public bool IsMatch(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }

            string[] filters = filter.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            return filters.All(f =>
            {
                return this.FirstName.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0
                    || this.LastName.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0
                    || this.Email.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0
                    || this.GitHub.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0
                    || this.TeamName.NullSafeToString().IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0;
            });
        }
    }
}