SELECT students.StudentId, students.FirstName, students.LastName, studentReviews.Week, studentReviews.Comments, studentreviews.StudentReviewId, studentreviews.Score, AVG(studentreviews.Score) OVER(PARTITION BY students.StudentId) AS AverageScore
FROM Students
LEFT JOIN StudentReviews ON students.StudentId = studentreviews.ReviewedStudentId
ORDER BY students.LastName, studentreviews.StudentReviewId;