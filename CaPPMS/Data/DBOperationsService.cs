using CaPPMS.Attributes;
using CaPPMS.Extensions;
using CaPPMS.Model;
using Humanizer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static MudBlazor.Defaults;

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
        /// Get Students List.
        /// </summary>
        /// <param name="classId">Class ID.</param>
        /// <returns>List of Students.</returns>
        public async Task<IEnumerable<Student>> RetrieveStudentsByClassAsync(long classId = -1)
        {
            var students = new List<Student>();
            string query = "SELECT * FROM Students";
            if (classId > -1)
            {
                query += " WHERE ClassId = @classId";
            }

            SqliteParameter classParam = new("@classId", classId);
            await ExecuteQueryAsync(
                query,
                (reader) =>
                {
                    students.Add(reader.ConvertRecord<Student>());
                },
                classParam);

            return students.AsReadOnly();
        }

        /// <summary>
        /// Get Students List.
        /// </summary>
        /// <param name="teamId">Team ID.</param>
        /// <returns>List of Students.</returns>
        public async Task<IEnumerable<Student>> RetrieveStudentsByTeamAsync(int teamId = -1)
        {
            var students = new List<Student>();
            string query = "SELECT * FROM Students";
            if (teamId > -1)
            {
                query += " WHERE TeamId = @teamId";
            }

            SqliteParameter teamParam = new("@teamId", teamId);
            await ExecuteQueryAsync(
                query,
                (reader) =>
                {
                    students.Add(reader.ConvertRecord<Student>());
                },
                teamParam);

            return students.AsReadOnly();
        }

        /// <summary>
        /// Get Student scores.
        /// </summary>
        /// <returns>List of scores.</returns>
        public async Task <IEnumerable<StudentScores>> RetrieveStudentScoresAsync()
        {
            List<StudentScores> studentScores = [];
            await ExecuteQueryAsync(
                readStudentScore,
                (record) => studentScores.Add(record.ConvertRecord<StudentScores>()));
            return studentScores.AsReadOnly();
        }

        /// <summary>
        /// Gets list of student scores by student id.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StudentScores>> RetrieveStudentScoresAsync(int studentId)
        {
            List<StudentScores> studentScores = [];
            SqliteParameter parameter = new("@studentId", studentId);
            await ExecuteQueryAsync(
                readStudentScore,
                (record) => studentScores.Add(record.ConvertRecord<StudentScores>()),
                parameter);
            return studentScores.AsReadOnly();
        }

        /// <summary>
        /// Get student score details.
        /// </summary>
        /// <returns>List of student scores.</returns>
        public async Task<IEnumerable<StudentScores>> RetrieveStudentScoreDetails()
        {
            List<StudentScores> studentScores = new List<StudentScores>();
            await ExecuteQueryAsync(
                readStudentScoreDetails,
                (record) => studentScores.Add(record.ConvertRecord<StudentScores>()));
            return studentScores.AsReadOnly();
        }

        /// <summary>
        /// Get a team list.
        /// </summary>
        /// <returns>List of teams.</returns>
        public async Task<IEnumerable<Team>> RetrieveTeamListAsync()
        {
            List<Team> teamList = [];
            string query = "SELECT * FROM Teams";
            await ExecuteQueryAsync(
                query,
                (record) => teamList.Add(record.ConvertRecord<Team>()));

            return teamList.AsReadOnly();
        }

        /// <summary>
        /// Get a team list.
        /// </summary>
        /// <returns>List of teams.</returns>
        public async Task<IEnumerable<ClassInformation>> RetrieveCohortAsync(long classId = -1)
        {
            List<ClassInformation> cohorts = [];
            string query = "SELECT * FROM ClassInformation";
            if (classId > -1)
            {
                query += " WHERE ClassId = @classId";
            }

            SqliteParameter parameter = new("@classId", classId);
            await ExecuteQueryAsync(
                query,
                (record) => cohorts.Add(record.ConvertRecord<ClassInformation>()),
                parameter);

            return cohorts.AsReadOnly();
        }

        /// <summary>
        /// Get number of weeks for the course.
        /// </summary>
        /// <returns></returns>
        public List<string> RetrieveWeeks()
        { 
            int weeks = this.ReteiveNumberOfWeeks();

            List<string> result = [];
            for (int i = 1; i <= weeks; i++)
            {
                result.Add(i.ToWords(WordForm.Normal).ApplyCase(LetterCasing.Sentence));
            }

            return result;
        }

        /// <summary>
        /// Gets the number of weeks a cohort is active.
        /// </summary>
        /// <returns>Default is 12 weeks, else what is configured.</returns>
        public int ReteiveNumberOfWeeks()
        {
            string numWeeks = Program.GetConfigurationSetting("CourseWeeks");
            if (string.IsNullOrEmpty(numWeeks))
            {
                numWeeks = "12";
            }

            return int.Parse(numWeeks);
        }

        /// <summary>
        /// Update Student to team assignment.
        /// </summary>
        /// <param name="studentId">Student ID.</param>
        /// <param name="teamId">Team ID</param>
        /// <returns>True if successful.</returns>
        public async Task<bool> UpdateTeamAssignmentAsync(long studentId, long teamId)
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
        public async Task<long> RetrieveUsersTeamAsync(string username)
        {
            long teamId = -1;
            string query = "SELECT TeamId FROM Students WHERE Email = @email";
            SqliteParameter parameter = new("@email", username);
            await ExecuteQueryAsync(
                query,
                (reader) =>
                {
                    teamId = reader[nameof(Team.TeamId)] == DBNull.Value ? -1 : Convert.ToInt64(reader[nameof(Team.TeamId)]);
                },
                parameter);

            return teamId;
        }

        /// <summary>
        /// Gets records of a type.
        /// </summary>
        /// <typeparam name="T">Type of record to get.</typeparam>
        /// <param name="id">Id of record, if not given, all records will be retrieved.</param>
        /// <returns><see cref="IEnumerable{T}"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IEnumerable<T>> GetRecords<T>(long id = -1) where T : class, new()
        {
            List<T> records = new();

            // Get the table name.
            string? tableName = typeof(T).GetCustomAttribute<SqlTableNameAttribute>()?.TableName;
            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException("Table name not found.");
            }

            // Build the query
            SqliteParameter parameter = new("@id", id);
            string query = $"SELECT * FROM {tableName};";
            if (id > -1)
            {
                query += " WHERE ClassId = @id";
            }

            // Execute
            await ExecuteQueryAsync(
                query,
                (record) => records.Add(record.ConvertRecord<T>()),
                parameter);
            return records.AsReadOnly();
        }

        public async Task<bool> AddRecord<T>(T record) where T : class, new()
        {
            ArgumentNullException.ThrowIfNull(record);

            // Get the table name.
            string? tableName = record.GetType().GetCustomAttribute<SqlTableNameAttribute>()?.TableName;
            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException("Table name not found.");
            }

            // Get the properties
            PropertyInfo[] properties = record.GetType().GetProperties();

            // Get the properties that are not the ID
            PropertyInfo[] nonIdProperties = properties
                .Where(prop =>
                {
                    return prop.GetCustomAttribute<SqlIdPropertyAttribute>() == null
                    && prop.GetCustomAttribute<IgnoreDataMemberAttribute>() == null;
                })
                .ToArray();

            // Build the query
            string query = $"INSERT INTO {tableName} (";
            string values = "VALUES (";
            List<SqliteParameter> parameters = new();
            foreach (PropertyInfo property in nonIdProperties)
            {
                query += $"{property.Name}, ";
                values += $"@{property.Name}, ";
                parameters.Add(new SqliteParameter($"@{property.Name}", property.GetValue(record)));
            }

            query = query.TrimEnd(',', ' ') + ") ";
            values = values.TrimEnd(',', ' ') + ");";
            query += values;
            // Execute
            int result = await ExecuteNonQueryAsync(query, [.. parameters]);
            return result > -1;
        }

        public async Task<bool> RemoveRecord<T>(T record)
        {
            ArgumentNullException.ThrowIfNull(record);

            // Get the table name.
            string? tableName = record.GetType().GetCustomAttribute<SqlTableNameAttribute>()?.TableName;

            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException("Table name not found.");
            }

            // Look for the ID property
            PropertyInfo? idProperty = record.GetType().GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<SqlIdPropertyAttribute>() != null);
            if (idProperty == null)
            {
                throw new InvalidOperationException("ID property not found.");
            }

            object? propertyValue = idProperty?.GetValue(record);
            if (propertyValue == null)
            {
                throw new InvalidOperationException("ID property value not found.");
            }

            // Execute
            string query = $"DELETE FROM {tableName} WHERE {idProperty?.Name} = @{idProperty?.Name};";
            List<SqliteParameter> parameters = new()
            {
                new SqliteParameter($"@{idProperty?.Name}", propertyValue)
            };
            int result = await ExecuteNonQueryAsync(query, [.. parameters]);
            return result > -1;
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

            FileInfo dbFileInfo = new (this.databaseFilePath);
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

        private async Task ExecuteQueryAsync(string query, Action<IDataReader> readerAction, params SqliteParameter[] sqliteParameters)
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

                        using (IDataReader reader = await command.ExecuteReaderAsync())
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing the database: {ex.Message}");
            }

            return result;
        }

        private async Task CreateDbAsync()
        {
            this.logger.LogDebug($"Creating database at {this.databaseFilePath}");
            string dbCreationScript = GetResourceData(StudentDataBaseCreation);
            try
            {
                int result = await ExecuteNonQueryAsync(dbCreationScript);
                this.logger.LogDebug($"Database creation result: {result > -1}");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error creating database. Error:{ex.GetBaseException()}");
            }
        }
    }
}