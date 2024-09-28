using Humanizer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CaPPMS.Data
{
    public class DBOperationsService
    {
        private const string ConnectionStringFormat = @"Data Source={0}";
        private const string RetriveStudentScoreDetailsFileName = "ReadStudentScoreDetails.sql";
        private const string ReadStudentScoresFileName = "ReadStudentScores.sql";
        private const string ReadStudentScoreByStudentFileName = "ReadStudentScoreByStudent.sql";
        private const string StudentDataBaseCreation = "StudentDataBaseCreation.sql";

        private static string readStudentScoreDetails;
        private static string readStudentScore;
        private static string readStudentScoreById;

        private static volatile int dbBroker;
        private static readonly TimeSpan brokerTimeout = TimeSpan.FromSeconds(10);

        static DBOperationsService()
        {
            readStudentScoreDetails = GetResourceData(RetriveStudentScoreDetailsFileName);
            readStudentScore = GetResourceData(ReadStudentScoresFileName);
            readStudentScoreById = GetResourceData(ReadStudentScoreByStudentFileName);
        }

        private string connectionString;
        private string databaseFilePath;
        private ILogger logger;

        public DBOperationsService(string dboperationsFilePath, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(dboperationsFilePath))
            {
                dboperationsFilePath = @"Data\StudentReviews.db";
            }

            // Normalize
            string fullFilePath = dboperationsFilePath.Replace('/', '\\');

            // Make sure it is a full path
            fullFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, fullFilePath.Trim('\\')));

            // Make sure full path is in expected location
            if (!fullFilePath.StartsWith(AppContext.BaseDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The database file path is not in the expected location.");
            }

            this.connectionString = string.Format(ConnectionStringFormat, fullFilePath);
            this.databaseFilePath = fullFilePath;
            this.logger = logger;
        }

        /// <summary>
        /// Get Students List
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <returns>List of Students</returns>
        public List<Student> RetrieveStudents(int teamId = -1)
        {
            var students = new List<Student>();
            string query = "SELECT StudentId, FirstName, LastName, TeamId FROM Students";
            if (teamId > -1)
            {
                query += " WHERE TeamId = @teamId";
            }

            SqliteParameter teamParam = new("@teamId", teamId);
            ExecuteQueryAsync(
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
        public List<StudentScores> RetrieveStudentScores()
        {
            List<StudentScores> studentScores = new();
            ExecuteQueryAsync(readStudentScore, (record) => studentScores.Add(ReadStudentRecord(record)));
            return studentScores;
        }

        /// <summary>
        /// Gets list of student scores by student id.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public List<StudentScores> RetrieveStudentScores(int studentId)
        {
            List<StudentScores> studentScores = new ();
            SqliteParameter parameter = new("@studentId", studentId);
            ExecuteQueryAsync(readStudentScore, (record) => studentScores.Add(ReadStudentRecord(record)), parameter);
            return studentScores;
        }

        /// <summary>
        /// Get student score details.
        /// </summary>
        /// <returns>List of student scores.</returns>
        public List<StudentScores> RetrieveStudentScoreDetails()
        {            
            List<StudentScores> studentScores = new List<StudentScores>();
            ExecuteQueryAsync(readStudentScoreDetails, (record) => studentScores.Add(ReadStudentRecord(record)));
            return studentScores;
        }

        /// <summary>
        /// Get a team list.
        /// </summary>
        /// <returns>List of teams.</returns>
        public List<Teams> RetrieveTeamList()
        {
            List<Teams> teamList = new List<Teams>();

            using (SqliteConnection connection = new(connectionString))
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
                                    Name = reader["TeamName"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error accessing the database: {ex.Message}");
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
        public List<string> RetrieveWeeks()
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
        public async Task<bool> UpdateTeamAssignmentAsync(int studentId, int teamId)
        {
            string query = "UPDATE Students Set TeamId = @teamId WHERE StudentId = @studentId";
            List<SqliteParameter> parameters =
            [
                new SqliteParameter("@teamId", teamId),
                new SqliteParameter("@studentId", studentId),
            ];
            int rowsAffected = await ExecuteNonQueryAsync(query, [.. parameters]);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Return team for student.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<int> RetrieveUsersTeamAsync(string username)
        {
            int teamId = -1;
            string query = "SELECT TeamId FROM Students WHERE Email = @email";
            SqliteParameter parameter = new ("@email", username);
            await ExecuteQueryAsync(
                query,
                (reader) =>
                {
                    teamId = reader["TeamId"] == DBNull.Value ? -1 : Convert.ToInt32(reader["TeamId"]);
                },
                parameter);

            return teamId;
        }

        /// <summary>
        /// Add Student to the database.
        /// </summary>
        /// <param name="student">Student to add.</param>
        public async Task<bool> AddStudent(Student student)
        {
            const string insertCommand = @"
INSERT INTO Students (FirstName, LastName, Email, TeamId)
VALUES (@FirstName, @LastName, @Email, @TeamId)";
            List<SqliteParameter> parameters =
            [
                new SqliteParameter("@FirstName", student.FirstName),
                new SqliteParameter("@LastName", student.LastName),
                new SqliteParameter("@Email", student.Email),
                new SqliteParameter("@TeamId", student.AssignedTeam.TeamId)
            ];

            int result = await ExecuteNonQueryAsync(insertCommand, [.. parameters]);

            if (result > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task EnsureDbExistsAsync()
        {
            DateTime timout = DateTime.Now.Add(brokerTimeout);
            while (Interlocked.CompareExchange(ref dbBroker, 1, 0) == 1)
            {
                await Task.Delay(100);

                if (DateTime.Now > timout)
                {
                    dbBroker = 0;
                    throw new InvalidOperationException("Access to DB not granted while trying to create. Blocked by previous request.");
                }
            }

            FileInfo dbFileInfo = new FileInfo(this.databaseFilePath);
            dbFileInfo.Directory?.Create();

            if (!dbFileInfo.Exists)
            {
                try
                {
                    await CreateDbAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error creating database. Error:{ex.GetBaseException()}");
                }
            }

            dbBroker = 0;
        }

        private async Task ExecuteQueryAsync(string query, Action<SqliteDataReader> readerAction, params SqliteParameter[] sqliteParameters)
        {
            using (SqliteConnection connection = new(connectionString))
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

                        using (SqliteDataReader reader = await command.ExecuteReaderAsync())
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

        private async Task<int> ExecuteNonQueryAsync(string query, params SqliteParameter[] parameters)
        {
            int result = -1;
            try
            {
                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();

                    using (SqliteCommand command = new(query, connection))
                    {
                        foreach (SqliteParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }

                        result = await command.ExecuteNonQueryAsync();
                    }

                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error accessing the database: {ex.Message}");
            }

            return result;
        }

        private StudentScores ReadStudentRecord(SqliteDataReader reader)
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

        private async Task CreateDbAsync()
        {
            this.logger.LogDebug($"Creating database at {this.databaseFilePath}");
            string dbCreationScript = GetResourceData(StudentDataBaseCreation);
            int result = await ExecuteNonQueryAsync(dbCreationScript);
            this.logger.LogDebug($"Database creation result: {result > -1}");
        }
    }
}