using CaPPMS.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace CaPPMS.Shared
{
    public class DBOperations
    {
        const string _connectionString = "Data Source=Data\\StudentReviews.db";

        public static List<Student> RetrieveStudents(int teamId = 0)
        {
            var students = new List<Student>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                try
                {
                    connection.Open();


                    string query = "SELECT StudentId, FirstName, LastName, TeamId FROM Students";

                    if (teamId > 0)
                    {
                        query += " WHERE TeamId = @teamId";
                    }

                    using (SqliteCommand command = new(query, connection))
                    {
                        if (teamId > 0)
                        {
                            command.Parameters.AddWithValue("@teamId", teamId);
                        }

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            Student student = null;

                            while (reader.Read())
                            {
                                student = new Student();
                                student.StudentId = Convert.ToInt32(reader["StudentId"]);
                                student.FirstName = reader["FirstName"].ToString();
                                student.LastName = reader["LastName"].ToString();
                                student.AssignedTeam.TeamId = Convert.ToInt32(reader["TeamId"]);
                                students.Add(student);
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

            return students;
        }

        public static List<StudentScores> RetrieveStudentScores()
        {
            List<StudentScores> studentScores = new List<StudentScores>();

            using (SqliteConnection connection = new(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT students.StudentId, students.FirstName, students.LastName, studentReviews.Week, studentReviews.Comments, " +
                        "studentreviews.StudentReviewId, studentreviews.Score, AVG(studentreviews.Score) " +
                        "OVER(PARTITION BY students.StudentId) AS AverageScore FROM Students " +
                        "LEFT JOIN StudentReviews ON students.StudentId = studentreviews.ReviewedStudentId " +
                        "ORDER BY students.LastName, studentreviews.StudentReviewId;";

                    using (SqliteCommand command = new(query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentScores.Add(new StudentScores()
                                {
                                    StudentId = Convert.ToInt32(reader["StudentId"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    AverageScore = reader["AverageScore"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AverageScore"]),
                                    Week = reader["Week"].ToString(),
                                    Score = reader["Score"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Score"]),
                                    Comments = reader["Comments"].ToString()
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

            return studentScores;
        }

        public static List<StudentScores> RetrieveStudentScores(int studentId)
        {
            List<StudentScores> studentScores = new List<StudentScores>();

            using (SqliteConnection connection = new(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT students.StudentId, students.FirstName, students.LastName, studentReviews.Week, studentReviews.Comments, " +
                        "studentreviews.StudentReviewId, studentreviews.Score, AVG(studentreviews.Score) " +
                        "OVER(PARTITION BY students.StudentId) AS AverageScore FROM Students " +
                        "LEFT JOIN StudentReviews ON students.StudentId = studentreviews.ReviewedStudentId WHERE students.StudentId = @studentId " +
                        "ORDER BY students.LastName;";

                    using (SqliteCommand command = new(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentScores.Add(new StudentScores()
                                {
                                    StudentId = Convert.ToInt32(reader["StudentId"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    AverageScore = reader["AverageScore"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AverageScore"]),
                                    Week = reader["Week"].ToString(),
                                    Score = reader["Score"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Score"]),
                                    Comments = reader["Comments"].ToString()
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

            return studentScores;
        }

        public static List<StudentScores> RetrieveStudentScoreDetails(int studentId)
        {
            List<StudentScores> studentScores = new List<StudentScores>();

            using (SqliteConnection connection = new(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
            WITH StudentAverages AS (
                SELECT ReviewedStudentId, AVG(Score) AS AverageScore
                FROM StudentReviews
                GROUP BY ReviewedStudentId
            )
            SELECT students.StudentId, students.FirstName, students.LastName,
                   studentreviews.Week, studentreviews.Score, studentreviews.Comments,
                   COALESCE(sa.AverageScore, 0) AS AverageScore
            FROM Students
            LEFT JOIN StudentReviews ON students.StudentId = studentreviews.ReviewedStudentId
            LEFT JOIN StudentAverages sa ON students.StudentId = sa.ReviewedStudentId
            ORDER BY students.LastName";

                    using (SqliteCommand command = new (query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentScores.Add(new StudentScores()
                                {
                                    StudentId = Convert.ToInt32(reader["StudentId"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Score = reader["Score"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Score"]),
                                    AverageScore = reader["AverageScore"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AverageScore"])
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

            return studentScores;
        }

        public static List<Teams> RetrieveTeamList()
        {
            List<Teams> teamList = new List<Teams>();

            using (SqliteConnection connection = new(_connectionString))
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

        public static List<string> RetrieveWeeks()
        {
            var tempList = new List<string>();

            using (SqliteConnection connection = new(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT WeekNumber FROM Week";

                    using (SqliteCommand command = new(query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tempList.Add(reader["WeekNumber"].ToString() ?? "");
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

            return tempList;
        }

        public static bool UpdateTeamAssignment(int studentId, int teamId)
        {
            bool updateSuccessful = false;

            try
            {
                string query = "UPDATE Students Set TeamId = @teamId WHERE StudentId = @studentId";

                using (SqliteConnection connection = new(_connectionString))
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
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }

            return updateSuccessful;
        }

        public static int RetrieveUsersTeam(string username)
        {
            int teamId = -1;

            using (SqliteConnection connection = new(_connectionString))
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

        public static void LoadStudent(Student student)
        {
            using (SqliteConnection connection = new(_connectionString))
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
    }
}