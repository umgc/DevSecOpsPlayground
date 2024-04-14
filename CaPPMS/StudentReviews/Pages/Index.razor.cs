using StudentReviews.Shared;
using StudentReviews.Data;
using System.Data.SQLite;
using System.ComponentModel.DataAnnotations;

namespace StudentReviews.Pages
{
    public partial class Index
    {
        const string _connectionString = "Data Source=Data\\StudentReviews.db";

        public Student student = new Student();

        public Review review = new Review();

        public List<CompletedReviews> completedReviews = new List<CompletedReviews>();

        public int count = -1;

        public bool isSubmitButtonDisabled = true;

        public List<Student> ClassList { get; set; }
 
        public List<string> WeeksList { get; set; }
 
        public string? SelectedStudent { get; set; }

        public string? log; 

        public string? StatusMessage;

        public bool HidePanel { get; set; } = true;

        private string username = "maria.stewart@yahoo.com";

        public Index()
        {
            int teamId = DBOperations.RetrieveUsersTeam(username);
            ClassList = DBOperations.RetrieveStudents(teamId);
            WeeksList = DBOperations.RetrieveWeeks();
        }

        public async Task SubmitEvaluation() //method returns the tasks now making it awaitable for SubmitEvaluation confirmation 
        {
            try
            {
                await Task.Run(() => //database operation now wrapped insided 'Task.Run' allowing to await method call -preserves the synchronous behavior while making method awaitable
                {
                    using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                    {
                        connection.Open();

                        string sql = @"INSERT INTO StudentReviews
                            (ReviewedStudentId, ReviewersEmail, Week, Score, Comments)
                            VALUES (@ReviewedStudentId, @ReviewersEmail, @Week, @Score, @Comments)";

                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
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
                                Student? foundStudent = ClassList.Find(student => student.StudentId == int.Parse(review.ReviewedStudentId));

                                if (foundStudent != null)
                                {
                                    completedReviews.Add(new CompletedReviews()
                                    {
                                        Week = review.Week,
                                        RatedStudent = LookupStudent(int.Parse(review.ReviewedStudentId)),
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

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT FirstName, LastName FROM Students WHERE StudentId = @studentId";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                student.FirstName = reader["FirstName"].ToString();
                                student.LastName = reader["LastName"].ToString();
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

        public string? SelectedWeek;
    }

    public class CompletedReviews
    {
        public string? RatedStudent { get; set; }

        public string? Week { get; set; }

        public string? Score { get; set; }

        public string? Comments { get; set; }
    }
}