CREATE TABLE [dbo].[FAQ]
(
	[FAQ_ID] INT NOT NULL PRIMARY KEY, 
    [FAQ_Question] NVARCHAR(50) NOT NULL, 
    [FAQ_Answer] NVARCHAR(MAX) NULL,
    [Author] NVARCHAR(MAX) NULL
)

CREATE TABLE [dbo].[Contact]
(
	[Contact_ID] BIGINT NOT NULL PRIMARY KEY, 
    [First_Name] NVARCHAR(50) NOT NULL, 
    [Last_Name] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(50) NULL, 
    [Phone] NCHAR(10) NULL
)

CREATE TABLE [dbo].[Project_Information]
(
	[P_ID] BIGINT NOT NULL PRIMARY KEY, 
    [Project_Title] NVARCHAR(50) NOT NULL, 
    [Project_Description] NVARCHAR(MAX) NULL, 
    [Project_Website] NCHAR(10) NULL, 
    [Project_Status] NCHAR(10) NULL
)

CREATE TABLE [dbo].[Comments]
(
	[Comment_ID] BIGINT NOT NULL PRIMARY KEY, 
    [Comment] NVARCHAR(MAX) NOT NULL, 
    [User] NVARCHAR(50) NOT NULL, 
    [DateTime_Added] DATETIME NOT NULL, 
    [P_ID] BIGINT NOT NULL, 
    CONSTRAINT [FK_Comments_Project_Information] FOREIGN KEY ([P_ID]) REFERENCES [Project_Information]([P_ID])
)

CREATE TABLE [dbo].[Attachment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [File_Name] NVARCHAR(50) NULL, 
    [File_Description] NVARCHAR(MAX) NULL, 
    [File_Path] NVARCHAR(MAX) NOT NULL, 
    [File_Size] BIGINT NULL, 
    [P_ID] BIGINT NOT NULL, 
    CONSTRAINT [FK_Attachment_Project_Information] FOREIGN KEY ([P_ID]) REFERENCES [Project_Information]([P_ID])
)

CREATE TABLE [dbo].[Project_Idea]
(
	[PI_ID] BIGINT NOT NULL PRIMARY KEY, 
    [P_ID] BIGINT NOT NULL, 
    [Submitted_ID] BIGINT NOT NULL, 
    [Sponsor_ID] BIGINT NOT NULL, 
    CONSTRAINT [FK_Project_Idea_Project_Information] FOREIGN KEY ([P_ID]) REFERENCES [Project_Information]([P_ID]), 
    CONSTRAINT [FK_Project_Idea_Contact] FOREIGN KEY ([Sponsor_ID]) REFERENCES [Contact]([Contact_ID])
)

CREATE TABLE [dbo].[Zip]
(
    [Zip_Id] BIGINT NOT NULL PRIMARY KEY, 
    [Zip_Name] NVARCHAR(50) NULL, 
    [Zip_Path] NVARCHAR(MAX) NOT NULL, 
    [Zip_Size] NVARCHAR(MAX) NULL,
    [Zip_Description] BIGINT NULL,
    [P_ID] BIGINT NOT NULL,
    [File_ID] BIGINT NOT NULL,
    CONSTRAINT [FK_Zip_Project_Information] FOREIGN KEY ([P_ID]) REFERENCES [Project_Information]([P_ID]),
    CONSTRAINT [FK_Zip_Attachment] FOREIGN KEY ([File_ID]) REFERENCES [Attachment]([File_ID])
)

CREATE TABLE [dbo].[Link]
(
    [Link_Id] BIGINT NOT NULL PRIMARY KEY, 
    [Link_URL] NVARCHAR(MAX) NOT NULL, 
    [P_ID] BIGINT NOT NULL,
    CONSTRAINT [FK_Link_Project_Information] FOREIGN KEY ([P_ID]) REFERENCES [Project_Information]([P_ID])
)


CREATE TABLE [dbo].[Team_Members]
(
    [TM_Id] BIGINT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NUL,  
    [P_ID] BIGINT NOT NULL,
    CONSTRAINT [FK_Team_members_Project_Information] FOREIGN KEY ([P_ID]) REFERENCES [Project_Information]([P_ID])
)

GO

CREATE INDEX [Index_Project_Title] ON [dbo].[Project_Information] ([Project_Title])
