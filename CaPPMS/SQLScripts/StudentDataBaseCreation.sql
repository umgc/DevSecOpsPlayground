CREATE TABLE "StudentReviews"
(
	"StudentReviewId"	INTEGER NOT NULL,
	"ReviewedStudentId"	INTEGER,
	"ReviewersEmail"	TEXT,
	"Week"	TEXT,
	"Score"	INTEGER,
	"Comments"	TEXT,
	PRIMARY KEY("StudentReviewId" AUTOINCREMENT)
);

CREATE TABLE "Students"
(
	"StudentId"	INTEGER,
	"FirstName"	TEXT NOT NULL,
	"LastName"	TEXT NOT NULL,
	"Email"	TEXT,
	"TeamId" INTEGER,
	"ClassId" INTEGER,
	PRIMARY KEY("StudentId" AUTOINCREMENT)
);

CREATE TABLE "Teams"
(
	"TeamId" INTEGER NOT NULL,
	"TeamName" TEXT NOT NULL,
	"ClassId" INTEGER NOT NULL,
	PRIMARY KEY("TeamId" AUTOINCREMENT)
);

CREATE TABLE "ClassInformation"
(
	"ClassId"	INTEGER NOT NULL,
	"Cohort"	TEXT NOT NULL,
	"Course" TEXT NOT NULL,
	"StartDate" Date,
	"EndDate" Date,
	PRIMARY KEY("ClassId" AUTOINCREMENT)
);