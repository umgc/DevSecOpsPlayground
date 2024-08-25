using Humanizer;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CaPPMS.Data
{
    public static class DBOperationsService
    {
        private const string ConnectionString = @"Data Source=Data\StudentReviews.db";
        private const string RetriveStudentScoreDetailsFileName = "ReadStudentScoreDetails.sql";
        private const string ReadStudentScoresFileName = "ReadStudentScores.sql";
        private const string ReadStudentScoreByStudentFileName = "ReadStudentScoreByStudent.sql";

        private static string readStudentScoreDetails;
        private static string readStudentScore;
        private static string readStudentScoreById;

        static DBOperationsService()
        {
            readStudentScoreDetails = GetResourceData(RetriveStudentScoreDetailsFileName);
            readStudentScore = GetResourceData(ReadStudentScoresFileName);
            readStudentScoreById = GetResourceData(ReadStudentScoreByStudentFileName);
        }

        /// <summary>
        /// Get Students List
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <returns>List of Students</returns>
        public static List<Student> RetrieveStudents(int teamId = -1)
        {
            var students = new List<Student>();
            string query = "SELECT StudentId, FirstName, LastName, TeamId FROM Students";
            if (teamId > -1)
            {
                query += " WHERE TeamId = @teamId";
            }

            SqliteParameter teamParam = new("@teamId", teamId);
            ExecuteQuery(
                query,
                (reader) =>
                {
                    var student = new Student();
                    student.StudentId = Convert.ToInt32(reader["StudentId"]);
                    student.FirstName = reader["FirstName"].NullSafeToString();
                    student.LastName = reader["LastName"].NullSafeToString();
                    student.AssignedTeam.TeamId = Convert.ToInt32(reader["TeamId"]);
                    students.Add(student);
                },
                teamParam);

            return students;
        }

        /// <summary>
        /// Get Student scores.
        /// </summary>
        /// <returns>List of scores.</returns>
        public static List<StudentScores> RetrieveStudentScores()
        {
            List<StudentScores> studentScores = new();
            ExecuteQuery(readStudentScore, (record) => studentScores.Add(ReadStudentRecord(record)));
            return studentScores;
        }

        /// <summary>
        /// Gets list of student scores by student id.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public static List<StudentScores> RetrieveStudentScores(int studentId)
        {
            List<StudentScores> studentScores = new ();
            SqliteParameter parameter = new("@studentId", studentId);
            ExecuteQuery(readStudentScore, (record) => studentScores.Add(ReadStudentRecord(record)), parameter);
            return studentScores;
        }

        /// <summary>
        /// Get student score details.
        /// </summary>
        /// <returns>List of student scores.</returns>
        public static List<StudentScores> RetrieveStudentScoreDetails()
        {            
            List<StudentScores> studentScores = new List<StudentScores>();
            ExecuteQuery(readStudentScoreDetails, (record) => studentScores.Add(ReadStudentRecord(record)));
            return studentScores;
        }

        /// <summary>
        /// Get a team list.
        /// </summary>
        /// <returns>List of teams.</returns>
        public static List<Teams> RetrieveTeamList()
        {
            List<Teams> teamList = new List<Teams>();

            using (SqliteConnection connection = new(ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT TeamId, TeamName FROM Teams";

                    using (SqliteCommand command = new(query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teamList.Add(new Teams()
                                {
                                    TeamId = Convert.ToInt32(reader["TeamId"]),
                                    Name = reader["TeamName"].ToString() ?? ""
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing the database: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return teamList;
        }

        /// <summary>
        /// Get number of weeks for the course.
        /// </summary>
        /// <returns></returns>
        public static List<string> RetrieveWeeks()
        {
            string numWeeks = Program.GetConfigurationSetting("CourseWeeks");

            if (string.IsNullOrEmpty(numWeeks))
            {
                numWeeks = "12";
            }

            int weeks = int.Parse(numWeeks);

            List<string> result = new List<string>();
            for (int i = 1; i <= weeks; i++)
            {
                result.Add(i.ToWords(WordForm.Normal).ApplyCase(LetterCasing.Sentence));
            }

            return result;
        }

        /// <summary>
        /// Update Student to team assignment.
        /// </summary>
        /// <param name="studentId">Student ID.</param>
        /// <param name="teamId">Team ID</param>
        /// <returns>True if successful.</returns>
        public static bool UpdateTeamAssignment(int studentId, int teamId)
        {
            bool updateSuccessful = false;

            try
            {
                string query = "UPDATE Students Set TeamId = @teamId WHERE StudentId = @studentId";

                using (SqliteConnection connection = new(ConnectionString))
                {
                    connection.Open();

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@teamId", teamId);
                        command.Parameters.AddWithValue("@studentId", studentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        updateSuccessful = rowsAffected > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return updateSuccessful;
        }

        /// <summary>
        /// Return team for student.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static int RetrieveUsersTeam(string username)
        {
            int teamId = -1;

            using (SqliteConnection connection = new(ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT TeamId FROM Students WHERE Email = @email";

                    using (SqliteCommand command = new(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", username);
                        teamId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing the database: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }

            return teamId;
        }

        /// <summary>
        /// Add Student to the database.
        /// </summary>
        /// <param name="student">Student to add.</param>
        public static void AddStudent(Student student)
        {
            using (SqliteConnection connection = new(ConnectionString))
            {
                connection.Open();

                var insertCommand = @"INSERT INTO Students (FirstName, LastName, Email, TeamId)
                VALUES (@FirstName, @LastName, @Email, @TeamId)";

                using (SqliteCommand command = new(insertCommand, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@TeamId", student.AssignedTeam.TeamId);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private static void ExecuteQuery(string query, Action<SqliteDataReader> readerAction, params SqliteParameter[] sqliteParameters)
        {
            using (SqliteConnection connection = new(ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (SqliteCommand command = new(query, connection))
                    {
                        foreach (SqliteParameter param in sqliteParameters)
                        {
                            command.Parameters.Add(param);
                        }

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                readerAction?.Invoke(reader);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing the database: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static StudentScores ReadStudentRecord(SqliteDataReader reader)
        {
            return new StudentScores(
                Convert.ToInt64(reader["StudentId"]),
                reader["FirstName"].NullSafeToString(),
                reader["LastName"].NullSafeToString())
            {
                AverageScore = reader["AverageScore"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AverageScore"]),
                Week = reader["Week"].NullSafeToString(),
                Score = reader["Score"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Score"]),
                Comment = reader["Comments"].NullSafeToString()
            };
        }

        private static string GetResourceData(string name)
        {
            string data = string.Empty;
            Assembly executing = Assembly.GetExecutingAssembly();
            string[] fileNames = executing.GetManifestResourceNames();
            foreach (string fileName in fileNames)
            {
                if (!fileName.EndsWith(name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!(executing.GetManifestResourceStream(fileName) is Stream stream))
                {
                    continue;
                }

                using (StreamReader sr = new StreamReader(stream))
                {
                    data = sr.ReadToEnd();
                }

                break;
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new InvalidDataException($"Expected to find the requested data but didn't or is empty. File:{name}");
            }

            return data;
        }
    }
}