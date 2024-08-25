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
ORDER BY students.LastName