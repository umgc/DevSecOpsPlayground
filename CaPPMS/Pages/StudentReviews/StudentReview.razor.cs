using CaPPMS.Extensions;
using CaPPMS.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaPPMS.Pages.StudentReviews
{
    public partial class StudentReview
    {
        const string _connectionString = "Data Source=Data\\StudentReviews.db";

        private string username = string.Empty;
        private string log = string.Empty;
        private int count = -1;
        private bool isSubmitButtonDisabled = true;
        private string SelectedWeek = string.Empty;

        public StudentReview()
        {
            //int teamId = this.bOperationsService.RetrieveUsersTeam(username);
            //ClassList = this.bOperationsService.RetrieveStudents(teamId);
            //WeeksList = this.bOperationsService.RetrieveWeeks();
        }

        public Student student = new Student();

        public Review review = new Review();

        public List<CompletedReviews> completedReviews = new List<CompletedReviews>();

        public List<Student>? ClassList { get; set; }

        public List<string>? WeeksList { get; set; }

        public string? StatusMessage { get; set; }

        public bool HidePanel { get; set; } = true;

        public async Task SubmitEvaluation() //method returns the tasks now making it awaitable for SubmitEvaluation confirmation 
        {
            try
            {
                await Task.Run(() => //database operation now wrapped insided 'Task.Run' allowing to await method call -preserves the synchronous behavior while making method awaitable
                {
                    using (SqliteConnection connection = new(_connectionString))
                    {
                        connection.Open();

                        string sql = @"INSERT INTO StudentReviews
                            (ReviewedStudentId, ReviewersEmail, Week, Score, Comments)
                            VALUES (@ReviewedStudentId, @ReviewersEmail, @Week, @Score, @Comments)";

                        using (SqliteCommand command = new(sql, connection))
                        {
                            command.Parameters.AddWithValue("@ReviewedStudentId", review.ReviewedStudentId);
                            command.Parameters.AddWithValue("@ReviewersEmail", username);
                            command.Parameters.AddWithValue("@Week", review.Week);
                            command.Parameters.AddWithValue("@Score", review.Score);
                            command.Parameters.AddWithValue("@Comments", review.Comments);

                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Student? foundStudent = ClassList?.Find(student => student.StudentId == review.ReviewedStudentId);

                                if (foundStudent != null)
                                {
                                    completedReviews.Add(new CompletedReviews()
                                    {
                                        Week = review.Week,
                                        RatedStudent = LookupStudent(review.ReviewedStudentId),
                                        Score = review.Score,
                                        Comments = review.Comments
                                    });
                                }

                                HidePanel = false;
                            }
                            else
                            {
                                Console.WriteLine("No rows affected. Data insertion failed.");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting data: {ex.Message}");
            }
        }

        public string LookupStudent(int studentId)
        {
            Student student = new Student();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT FirstName, LastName FROM Students WHERE StudentId = @studentId";

                    using (SqliteCommand command = new(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                student.FirstName = reader["FirstName"].NullSafeToString();
                                student.LastName = reader["LastName"].NullSafeToString();
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

            return student.FirstName + " " + student.LastName;
        }

    }

    public struct CompletedReviews
    {
        public string RatedStudent { get; set; }

        public string Week { get; set; }

        public string Score { get; set; }

        public string Comments { get; set; }
    }
}