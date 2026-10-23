/*
    CV Management System - SQL Server
    Database: CVManagementDB
    Target: SQL Server 2019+ (should also work on recent SQL Server Express editions)

    Notes:
    - PasswordHash is for a hashed password only. Never store plaintext passwords.
    - Store CV files in file/object storage; save their metadata and storage key here.
    - Seeded roles are illustrative. Configure ASP.NET Core Identity/JWT securely in the API.
*/

IF DB_ID(N'CVManagementDB') IS NULL
BEGIN
    CREATE DATABASE CVManagementDB;
END
GO

USE CVManagementDB;
GO

/* =========================
   1. Roles and user accounts
   ========================= */
CREATE TABLE dbo.Roles
(
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    Name            NVARCHAR(50) NOT NULL,
    Description     NVARCHAR(250) NULL,
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.Users
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    RoleId          INT NOT NULL,
    Email           NVARCHAR(256) NOT NULL,
    PasswordHash    NVARCHAR(500) NOT NULL,
    FullName        NVARCHAR(150) NOT NULL,
    Phone           NVARCHAR(30) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

/* =========================
   2. Candidate profile
   ========================= */
CREATE TABLE dbo.Candidates
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Candidates PRIMARY KEY,
    UserId          BIGINT NOT NULL,
    DateOfBirth     DATE NULL,
    Address         NVARCHAR(300) NULL,
    LinkedInUrl     NVARCHAR(500) NULL,
    PortfolioUrl    NVARCHAR(500) NULL,
    Summary         NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Candidates_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_Candidates_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
    CONSTRAINT UQ_Candidates_UserId UNIQUE (UserId)
);
GO

/* =========================
   3. CVs / resumes
   ========================= */
CREATE TABLE dbo.Resumes
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Resumes PRIMARY KEY,
    CandidateId     BIGINT NOT NULL,
    Title           NVARCHAR(150) NOT NULL,
    OriginalFileName NVARCHAR(255) NOT NULL,
    StorageKey      NVARCHAR(1000) NOT NULL,
    ContentType     NVARCHAR(100) NOT NULL CONSTRAINT DF_Resumes_ContentType DEFAULT (N'application/pdf'),
    FileSizeBytes   BIGINT NULL,
    IsDefault       BIT NOT NULL CONSTRAINT DF_Resumes_IsDefault DEFAULT (0),
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Resumes_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_Resumes_Candidates FOREIGN KEY (CandidateId) REFERENCES dbo.Candidates(Id),
    CONSTRAINT UQ_Resumes_Id_CandidateId UNIQUE (Id, CandidateId),
    CONSTRAINT CK_Resumes_FileSize CHECK (FileSizeBytes IS NULL OR FileSizeBytes >= 0)
);
GO

CREATE INDEX IX_Resumes_CandidateId_CreatedAt
ON dbo.Resumes(CandidateId, CreatedAt DESC);
GO

/* =========================
   4. Jobs and job skills
   ========================= */
CREATE TABLE dbo.JobPosts
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_JobPosts PRIMARY KEY,
    CreatedByUserId BIGINT NOT NULL,
    Title           NVARCHAR(200) NOT NULL,
    Department      NVARCHAR(100) NULL,
    Location        NVARCHAR(200) NULL,
    EmploymentType  NVARCHAR(50) NULL,
    ExperienceMinYears TINYINT NULL,
    SalaryMin       DECIMAL(18,2) NULL,
    SalaryMax       DECIMAL(18,2) NULL,
    Currency        CHAR(3) NOT NULL CONSTRAINT DF_JobPosts_Currency DEFAULT ('VND'),
    Description     NVARCHAR(MAX) NOT NULL,
    Requirements   NVARCHAR(MAX) NULL,
    Benefits        NVARCHAR(MAX) NULL,
    Status          NVARCHAR(20) NOT NULL CONSTRAINT DF_JobPosts_Status DEFAULT ('Draft'),
    PublishedAt     DATETIME2(0) NULL,
    Deadline        DATE NULL,
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_JobPosts_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_JobPosts_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_JobPosts_Experience CHECK (ExperienceMinYears IS NULL OR ExperienceMinYears >= 0),
    CONSTRAINT CK_JobPosts_Salary CHECK
        (SalaryMin IS NULL OR SalaryMax IS NULL OR SalaryMin <= SalaryMax),
    CONSTRAINT CK_JobPosts_Status CHECK (Status IN ('Draft', 'Published', 'Closed', 'Archived'))
);
GO

CREATE INDEX IX_JobPosts_Status_Deadline
ON dbo.JobPosts(Status, Deadline);
GO

CREATE TABLE dbo.Skills
(
    Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Skills PRIMARY KEY,
    Name            NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Skills_Name UNIQUE (Name)
);
GO

CREATE TABLE dbo.CandidateSkills
(
    CandidateId     BIGINT NOT NULL,
    SkillId         INT NOT NULL,
    YearsOfExperience DECIMAL(4,1) NULL,
    ProficiencyLevel NVARCHAR(30) NULL,
    CONSTRAINT PK_CandidateSkills PRIMARY KEY (CandidateId, SkillId),
    CONSTRAINT FK_CandidateSkills_Candidates FOREIGN KEY (CandidateId) REFERENCES dbo.Candidates(Id),
    CONSTRAINT FK_CandidateSkills_Skills FOREIGN KEY (SkillId) REFERENCES dbo.Skills(Id),
    CONSTRAINT CK_CandidateSkills_Years CHECK (YearsOfExperience IS NULL OR YearsOfExperience >= 0),
    CONSTRAINT CK_CandidateSkills_Level CHECK
        (ProficiencyLevel IS NULL OR ProficiencyLevel IN ('Beginner', 'Intermediate', 'Advanced', 'Expert'))
);
GO

CREATE TABLE dbo.JobSkills
(
    JobPostId       BIGINT NOT NULL,
    SkillId         INT NOT NULL,
    IsRequired      BIT NOT NULL CONSTRAINT DF_JobSkills_IsRequired DEFAULT (1),
    CONSTRAINT PK_JobSkills PRIMARY KEY (JobPostId, SkillId),
    CONSTRAINT FK_JobSkills_JobPosts FOREIGN KEY (JobPostId) REFERENCES dbo.JobPosts(Id),
    CONSTRAINT FK_JobSkills_Skills FOREIGN KEY (SkillId) REFERENCES dbo.Skills(Id)
);
GO

/* =========================
   5. Applications
   ========================= */
CREATE TABLE dbo.Applications
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Applications PRIMARY KEY,
    CandidateId     BIGINT NOT NULL,
    JobPostId       BIGINT NOT NULL,
    ResumeId        BIGINT NOT NULL,
    CoverLetter     NVARCHAR(MAX) NULL,
    Status          NVARCHAR(30) NOT NULL CONSTRAINT DF_Applications_Status DEFAULT ('Submitted'),
    AppliedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Applications_AppliedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_Applications_Candidates FOREIGN KEY (CandidateId) REFERENCES dbo.Candidates(Id),
    CONSTRAINT FK_Applications_JobPosts FOREIGN KEY (JobPostId) REFERENCES dbo.JobPosts(Id),
    CONSTRAINT FK_Applications_Resumes_Candidate
        FOREIGN KEY (ResumeId, CandidateId) REFERENCES dbo.Resumes(Id, CandidateId),
    CONSTRAINT UQ_Applications_Candidate_Job UNIQUE (CandidateId, JobPostId),
    CONSTRAINT CK_Applications_Status CHECK
        (Status IN ('Submitted', 'Screening', 'Interviewing', 'Offered', 'Hired', 'Rejected', 'Withdrawn'))
);
GO

CREATE INDEX IX_Applications_JobPost_Status_AppliedAt
ON dbo.Applications(JobPostId, Status, AppliedAt DESC);
GO

CREATE INDEX IX_Applications_Candidate_AppliedAt
ON dbo.Applications(CandidateId, AppliedAt DESC);
GO

/* =========================
   6. Application status history
   ========================= */
CREATE TABLE dbo.ApplicationHistories
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ApplicationHistories PRIMARY KEY,
    ApplicationId   BIGINT NOT NULL,
    ChangedByUserId BIGINT NOT NULL,
    OldStatus       NVARCHAR(30) NULL,
    NewStatus       NVARCHAR(30) NOT NULL,
    Note            NVARCHAR(1000) NULL,
    ChangedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_ApplicationHistories_ChangedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_ApplicationHistories_Applications FOREIGN KEY (ApplicationId) REFERENCES dbo.Applications(Id),
    CONSTRAINT FK_ApplicationHistories_Users FOREIGN KEY (ChangedByUserId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_ApplicationHistories_NewStatus CHECK
        (NewStatus IN ('Submitted', 'Screening', 'Interviewing', 'Offered', 'Hired', 'Rejected', 'Withdrawn')),
    CONSTRAINT CK_ApplicationHistories_OldStatus CHECK
        (OldStatus IS NULL OR OldStatus IN ('Submitted', 'Screening', 'Interviewing', 'Offered', 'Hired', 'Rejected', 'Withdrawn'))
);
GO

CREATE INDEX IX_ApplicationHistories_ApplicationId_ChangedAt
ON dbo.ApplicationHistories(ApplicationId, ChangedAt DESC);
GO

/* =========================
   7. Interviews
   ========================= */
CREATE TABLE dbo.Interviews
(
    Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Interviews PRIMARY KEY,
    ApplicationId   BIGINT NOT NULL,
    InterviewerUserId BIGINT NOT NULL,
    ScheduledAt     DATETIME2(0) NOT NULL,
    DurationMinutes SMALLINT NOT NULL CONSTRAINT DF_Interviews_Duration DEFAULT (60),
    Location        NVARCHAR(300) NULL,
    MeetingUrl      NVARCHAR(1000) NULL,
    Result          NVARCHAR(30) NULL,
    Feedback        NVARCHAR(MAX) NULL,
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Interviews_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(0) NULL,
    CONSTRAINT FK_Interviews_Applications FOREIGN KEY (ApplicationId) REFERENCES dbo.Applications(Id),
    CONSTRAINT FK_Interviews_Users FOREIGN KEY (InterviewerUserId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_Interviews_Duration CHECK (DurationMinutes > 0),
    CONSTRAINT CK_Interviews_Result CHECK
        (Result IS NULL OR Result IN ('Pending', 'Passed', 'Failed', 'Rescheduled', 'Cancelled'))
);
GO

CREATE INDEX IX_Interviews_Interviewer_ScheduledAt
ON dbo.Interviews(InterviewerUserId, ScheduledAt);
GO

CREATE INDEX IX_Interviews_ApplicationId_ScheduledAt
ON dbo.Interviews(ApplicationId, ScheduledAt);
GO

/* =========================
   8. Seed basic roles and skills
   ========================= */
INSERT INTO dbo.Roles(Name, Description)
VALUES
    (N'Admin', N'Quản trị hệ thống'),
    (N'HR', N'Nhân sự tuyển dụng'),
    (N'Candidate', N'Ứng viên');
GO

INSERT INTO dbo.Skills(Name)
VALUES
    (N'C#'),
    (N'.NET'),
    (N'ASP.NET Core Web API'),
    (N'SQL Server'),
    (N'Entity Framework Core'),
    (N'Angular'),
    (N'JavaScript'),
    (N'HTML/CSS'),
    (N'Git'),
    (N'Docker');
GO

/* =========================
   9. Quick verification
   ========================= */
SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO
