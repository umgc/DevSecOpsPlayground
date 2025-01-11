using CaPPMS.Attributes;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    [SqlTableName("Students")]
    public class Student
    {
        private long classId = -1;

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
        [DisplayName("Team")]
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

        public long ClassId
        {
            get
            {
                return this.classId;
            }
            set
            {
                this.classId = value;
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
        public static List<Student> ParseData(string data)
        {
            List<Student> students = new List<Student>();

            try
            {
                // Split the data into lines
                string[] lines = data.Split('\n');

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
                    student.Email = fields[headerMap["Email"]];

                    // Add the student to the list
                    students.Add(student);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return students;
        }
    }
}