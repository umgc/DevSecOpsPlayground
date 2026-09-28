CREATE TABLE "StudentReviews"
(
	"StudentReviewId"	INTEGER NOT NULL,
	"ReviewedById"	INTEGER,
	"ReviewedStudentId"	INTEGER,
	"Week"	TEXT,
	"Score"	TEXT,
	"Comments"	TEXT,
	PRIMARY KEY("StudentReviewId" AUTOINCREMENT)
);

CREATE TABLE "Students"
(
	"StudentId"	INTEGER,
	"FirstName"	TEXT NOT NULL,
	"LastName"	TEXT NOT NULL,
	"Email"	TEXT,
	"GitHub" TEXT,
	"TeamId" INTEGER,
	"ClassId" INTEGER,
	PRIMARY KEY("StudentId" AUTOINCREMENT)
);

CREATE TABLE "Teams"
(
	"TeamId" INTEGER NOT NULL,
	"Name" TEXT NOT NULL,
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