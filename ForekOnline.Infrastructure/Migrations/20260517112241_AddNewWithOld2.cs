using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewWithOld2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FO");

            migrationBuilder.EnsureSchema(
                name: "Academics");

            migrationBuilder.EnsureSchema(
                name: "Finance");

            migrationBuilder.EnsureSchema(
                name: "Alerts");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Line1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssociativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationCycle",
                schema: "FO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    OpensAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Closes = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationPeriod = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TurnaroundDays = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCycle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationEvent",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllDay = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationEvent", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationSubmissionQueue",
                schema: "FO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnlineApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    LockedUntilUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreatedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProcessedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AcknowledgementSentUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSubmissionQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentAttachments",
                columns: table => new
                {
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Module = table.Column<int>(type: "int", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FileURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAttachments", x => x.AttachmentId);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearnerIdPass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FinalScore = table.Column<int>(type: "int", nullable: true),
                    Percentage = table.Column<double>(type: "float", nullable: true),
                    FocusLossCount = table.Column<int>(type: "int", nullable: false),
                    WasFullscreenViolated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAttempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    TimerMinutes = table.Column<int>(type: "int", nullable: false),
                    IsPasswordProtected = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowRetries = table.Column<bool>(type: "bit", nullable: false),
                    MaxRetryAttempts = table.Column<int>(type: "int", nullable: true),
                    ShuffleQuestions = table.Column<bool>(type: "bit", nullable: false),
                    ShuffleAnswers = table.Column<bool>(type: "bit", nullable: false),
                    ShowReviewAfter = table.Column<bool>(type: "bit", nullable: false),
                    EnforceFullscreen = table.Column<bool>(type: "bit", nullable: false),
                    MaxFocusLossAllowed = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssessmentType = table.Column<int>(type: "int", nullable: false),
                    EnableMathRendering = table.Column<bool>(type: "bit", nullable: false),
                    IsModerated = table.Column<bool>(type: "bit", nullable: false),
                    ModeratedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeratedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModerationApproved = table.Column<bool>(type: "bit", nullable: true),
                    ModerationRejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackgroundJobQueueItem",
                schema: "FO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Queue = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    LockedUntilUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundJobQueueItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactPerson",
                columns: table => new
                {
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPerson", x => x.ContactId);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    NType = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<double>(type: "float", nullable: true),
                    NQFLevel = table.Column<int>(type: "int", nullable: true),
                    IsEligibleForOnlineApplications = table.Column<bool>(type: "bit", nullable: false),
                    MinimumRequirement = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Student = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RequestPurpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsEmailIssued = table.Column<bool>(type: "bit", nullable: false),
                    IsHardCopyIssued = table.Column<bool>(type: "bit", nullable: false),
                    DocumentUpload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedStudentIDs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeContact",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCellNumberVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeContact", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "Evidence",
                columns: table => new
                {
                    EvidenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Module = table.Column<int>(type: "int", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidence", x => x.EvidenceId);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BlobFileURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileId);
                });

            migrationBuilder.CreateTable(
                name: "Finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatementName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForekBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForekBase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InAppNotification",
                schema: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconCss = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InAppNotification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeftUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    LastEventUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonAttendances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonPlan",
                columns: table => new
                {
                    LessonPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Course = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Module = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: true),
                    Funder = table.Column<int>(type: "int", nullable: false),
                    Approval = table.Column<int>(type: "int", nullable: false),
                    IsApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlan", x => x.LessonPlanId);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JoinUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    LicenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<int>(type: "int", nullable: false),
                    CourseKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfIssue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfExpiry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileUpload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientType = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.LicenseId);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Trade = table.Column<int>(type: "int", nullable: false),
                    Module = table.Column<int>(type: "int", nullable: true),
                    InstructorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.MaterialId);
                });

            migrationBuilder.CreateTable(
                name: "Medicals",
                columns: table => new
                {
                    MedicalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicals", x => x.MedicalId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderIconCss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderGradientCss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderTextColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModalSize = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AudienceRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarouselGroupKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnlineApplication",
                schema: "FO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    CourseTypeCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    FundingTypeCode = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    FirstChoiceCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SecondChoiceCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThirdChoiceCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubmittedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSavedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TatDays = table.Column<int>(type: "int", nullable: true),
                    HasExceededTurnaroundTime = table.Column<bool>(type: "bit", nullable: false),
                    LastTatEvaluatedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OverdueNotifiedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineApplication", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnlineApplicationUser",
                schema: "FO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayslipRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartMonth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndMonth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayslipUploaded = table.Column<bool>(type: "bit", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdPass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportType = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Challenges = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacilitatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urgency = table.Column<int>(type: "int", nullable: true),
                    Operation = table.Column<int>(type: "int", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    OpenCount = table.Column<int>(type: "int", nullable: false),
                    LastOpened = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasExpired = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsLiked = table.Column<bool>(type: "bit", nullable: true),
                    LastDownloaded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    IsLateSubmission = table.Column<bool>(type: "bit", nullable: false),
                    IntendedPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IntendedPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IntendedPeriodLabel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LateAcknowledgedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                });

            migrationBuilder.CreateTable(
                name: "ReportSubReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastDownloaded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSubReport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Course = table.Column<int>(type: "int", nullable: false),
                    ReportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ReportId);
                });

            migrationBuilder.CreateTable(
                name: "StorageSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConnectionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxSizeMB = table.Column<int>(type: "int", nullable: false),
                    EncryptAtRest = table.Column<bool>(type: "bit", nullable: false),
                    Compress = table.Column<bool>(type: "bit", nullable: false),
                    AllowedMimeTypesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetentionDays = table.Column<int>(type: "int", nullable: false),
                    FallbackProviderType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomMetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    EnablePresignedUrls = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttachments",
                columns: table => new
                {
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentName = table.Column<int>(type: "int", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttachments", x => x.AttachmentId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "Academics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StudyPermitNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AdmissionCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StreetAddressLine1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StreetAddressLine2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Cellphone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    HighestGrade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameOfSchool = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeregistered = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationSource = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OriginalApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdPassportDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppTitle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FaviconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    PrimaryColorLight = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    PrimaryColorDark = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    TextColor = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    PhysicalAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FacebookUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CopyrightHolder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Training",
                columns: table => new
                {
                    TrainingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training", x => x.TrainingId);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LoginTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LogoutTimeUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastActivityUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsCurrentSession = table.Column<bool>(type: "bit", nullable: false),
                    ForceLogoutPerformed = table.Column<bool>(type: "bit", nullable: true),
                    LogoutReason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    Department = table.Column<int>(type: "int", nullable: true),
                    ActivationCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResetPasswordCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cellphone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IDPass = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: true),
                    LastActivityDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailSignatureLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHeadOfDepartment = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Departments = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    VenueType = table.Column<int>(type: "int", nullable: false),
                    EquipmentChecklist = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultBookingRules = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FloorPlanUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HasReport = table.Column<bool>(type: "bit", nullable: false),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SelectedEmployeeIDs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LearnerFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitPurpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mentor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.VisitId);
                });

            migrationBuilder.CreateTable(
                name: "WorkplaceModules",
                columns: table => new
                {
                    LearnerWorkplaceModulesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Student = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkplaceModules", x => x.LearnerWorkplaceModulesId);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Selection = table.Column<int>(type: "int", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyPermitCategory = table.Column<int>(type: "int", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantAddressAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantSurname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantTitle = table.Column<int>(type: "int", nullable: false),
                    IDPassFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighestQualFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResidenceFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    HighestQualification = table.Column<int>(type: "int", nullable: false),
                    FunderType = table.Column<int>(type: "int", nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDPassDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HighestQualDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidenceDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PendingStatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PendingStatusMessage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Address_ApplicantAddressAddressId",
                        column: x => x.ApplicantAddressAddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentAttemptAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShortAnswerValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    MarksAwarded = table.Column<int>(type: "int", nullable: true),
                    DiagramAnnotationJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagramAnnotationSnapshotFileId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentAttemptAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentAttemptAnswers_AssessmentAttempts_AssessmentAttemptId",
                        column: x => x.AssessmentAttemptId,
                        principalTable: "AssessmentAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableAnnotation = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Marks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentQuestions_Assessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Speciality = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Company_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Company_ContactPerson_ContactId",
                        column: x => x.ContactId,
                        principalTable: "ContactPerson",
                        principalColumn: "ContactId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CourseIdFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NQFLevel = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<double>(type: "float", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_Module_Course_CourseIdFK",
                        column: x => x.CourseIdFK,
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationContentBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListItems = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AltText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationContentBlocks_NotificationEvents_NotificationEventId",
                        column: x => x.NotificationEventId,
                        principalTable: "NotificationEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FileURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoredFileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoredFileProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TagsCsv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewsCount = table.Column<int>(type: "int", nullable: false),
                    DownloadsCount = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_ResourceCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ResourceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredDocumentContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredDocumentContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredDocumentContents_StoredDocuments_Id",
                        column: x => x.Id,
                        principalTable: "StoredDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                schema: "Academics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CourseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnrollmentStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Academics",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: true),
                    MaxStudents = table.Column<int>(type: "int", nullable: true),
                    StartsOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SuspensionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSubscriptions_TenantProfiles_TenantProfileId",
                        column: x => x.TenantProfileId,
                        principalTable: "TenantProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VenueReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilitatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilitatorName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Campus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpectedStudents = table.Column<int>(type: "int", nullable: false),
                    ReservedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionedByHodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionedByHodName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueReservations_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialClearance",
                schema: "Finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AmountExpected = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountReceived = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProofOfPaymentFileId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProofOfPaymentFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    UserCreated = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialClearance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialClearance_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    GuardianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Relationship = table.Column<int>(type: "int", nullable: false),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDDoc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.GuardianId);
                    table.ForeignKey(
                        name: "FK_Guardians_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RejectionTBL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    NextSteps = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectionTBL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RejectionTBL_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQuestionOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentQuestionOptions_AssessmentQuestions_AssessmentQuestionId",
                        column: x => x.AssessmentQuestionId,
                        principalTable: "AssessmentQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Placements",
                columns: table => new
                {
                    PlacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Student = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlacedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Module = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    WorkplaceMentorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkplaceMentorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkplaceMentorPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacementAgreement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DigitalSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Placements", x => x.PlacementId);
                    table.ForeignKey(
                        name: "FK_Placements_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceRoleAudiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceRoleAudiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceRoleAudiences_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceUserAudiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceUserAudiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceUserAudiences_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceUserAudiences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VenueAssessmentBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssessmentType = table.Column<int>(type: "int", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    StudentList = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailsSent = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueAssessmentBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueAssessmentBookings_VenueReservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "VenueReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VenueReservationAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserModified = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueReservationAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenueReservationAudits_VenueReservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "VenueReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NotificationEvents",
                columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
                values: new object[,]
                {
                    { new Guid("2ec20796-c29a-4ea5-b98f-c45d714f3193"), null, null, 1, new DateTime(2026, 5, 24, 13, 22, 40, 440, DateTimeKind.Local).AddTicks(2559), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 5, 16, 13, 22, 40, 440, DateTimeKind.Local).AddTicks(2537), "Forek Online Version 2 Highlights" },
                    { new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), null, null, 1, new DateTime(2026, 5, 24, 11, 22, 40, 440, DateTimeKind.Utc).AddTicks(3768), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 5, 16, 11, 22, 40, 440, DateTimeKind.Utc).AddTicks(3768), "Forek Online Version 2 Highlights" }
                });

            migrationBuilder.InsertData(
                table: "TenantProfiles",
                columns: new[] { "Id", "AccentColor", "AppTitle", "BackgroundColor", "Code", "ContactEmail", "ContactPhone", "CopyrightHolder", "DateDeleted", "FacebookUrl", "FaviconUrl", "InstagramUrl", "IsDeleted", "LogoUrl", "Name", "PhysicalAddress", "PrimaryColor", "PrimaryColorDark", "PrimaryColorLight", "Tagline", "TextColor", "TwitterUrl", "UserCreated", "UserModified", "WebsiteUrl", "YouTubeUrl" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "#ff9100", "Forek Online", "#fafafa", "DEFAULT", "info@forek.co.za", null, "Forek ICT Services", null, "https://www.facebook.com/ForekInstitute", "/favicon.png", "https://www.instagram.com/forekinstituteoftechnology/", false, "/Theme/images/logob-removebg-preview.png", "Default Tenant Profile", "Factory 107 Kabokweni, Industrial", "#e65100", "#ac1900", "#ff8a50", "Powered By Forek ICT Services", "#1a1a2e", "https://twitter.com/forekinstitute", null, null, "https://www.forek.co.za", "https://www.youtube.com/@forekinstituteoftechnology2257" });

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[,]
                {
                    { new Guid("8c43c1e8-64db-4a41-b087-bfc97dae5c9d"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 },
                    { new Guid("9f94d0ed-c4ad-4f0e-9127-ab4a9d8b1b4a"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 },
                    { new Guid("b1c7d4a2-75fc-46cb-8b5b-386593cfd598"), null, null, null, new Guid("2ec20796-c29a-4ea5-b98f-c45d714f3193"), 0, null, "A sleeker, faster platform...", 0 },
                    { new Guid("b6f4594c-dffa-4723-899d-01936da8de47"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationCycle_AcademicYear",
                schema: "FO",
                table: "ApplicationCycle",
                column: "AcademicYear");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationCycle_Year_Period",
                schema: "FO",
                table: "ApplicationCycle",
                columns: new[] { "AcademicYear", "ApplicationPeriod" });

            migrationBuilder.CreateIndex(
                name: "UX_ApplicationCycle_Year_Period_Active",
                schema: "FO",
                table: "ApplicationCycle",
                columns: new[] { "AcademicYear", "ApplicationPeriod", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicantAddressAddressId",
                table: "Applications",
                column: "ApplicantAddressAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentAttemptAnswers_AssessmentAttemptId",
                table: "AssessmentAttemptAnswers",
                column: "AssessmentAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestionOptions_AssessmentQuestionId",
                table: "AssessmentQuestionOptions",
                column: "AssessmentQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQuestions_AssessmentId",
                table: "AssessmentQuestions",
                column: "AssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundJobQueueItem_DateCreated",
                schema: "FO",
                table: "BackgroundJobQueueItem",
                column: "DateCreated");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundJobQueueItem_JobType",
                schema: "FO",
                table: "BackgroundJobQueueItem",
                column: "JobType");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundJobQueueItem_Q_Status_Lock_Created",
                schema: "FO",
                table: "BackgroundJobQueueItem",
                columns: new[] { "Queue", "Status", "LockedUntilUtc", "DateCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundJobQueueItem_Queue_Status_Lock",
                schema: "FO",
                table: "BackgroundJobQueueItem",
                columns: new[] { "Queue", "Status", "LockedUntilUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_AddressId",
                table: "Company",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_ContactId",
                table: "Company",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_IsActive",
                schema: "Academics",
                table: "Enrollments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_Enrollment_Student_Course",
                schema: "Academics",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialClearance_ApplicationId",
                schema: "Finance",
                table: "FinancialClearance",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_ApplicationId",
                table: "Guardians",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InAppNotification_Recipient_Read_Created",
                schema: "Alerts",
                table: "InAppNotification",
                columns: new[] { "RecipientUserId", "IsRead", "DateCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttendances_LessonId_StudentId",
                table: "LessonAttendances",
                columns: new[] { "LessonId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_RoomName",
                table: "Lessons",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseIdFK",
                table: "Module",
                column: "CourseIdFK");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationContentBlocks_NotificationEventId",
                table: "NotificationContentBlocks",
                column: "NotificationEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Placements_CompanyId",
                table: "Placements",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RejectionTBL_ApplicationId",
                table: "RejectionTBL",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubReport_ReportId",
                table: "ReportSubReport",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_CategoryId",
                table: "Resource",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRoleAudiences_ResourceId_Role",
                table: "ResourceRoleAudiences",
                columns: new[] { "ResourceId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUserAudiences_ResourceId_UserId",
                table: "ResourceUserAudiences",
                columns: new[] { "ResourceId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUserAudiences_UserId",
                table: "ResourceUserAudiences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_Email",
                schema: "Academics",
                table: "Students",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Student_IDNumber",
                schema: "Academics",
                table: "Students",
                column: "IDNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Student_PassportNumber",
                schema: "Academics",
                table: "Students",
                column: "PassportNumber");

            migrationBuilder.CreateIndex(
                name: "UX_Student_StudentNumber",
                schema: "Academics",
                table: "Students",
                column: "StudentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscription_ExpiresOn",
                table: "TenantSubscriptions",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscription_Tenant_Status",
                table: "TenantSubscriptions",
                columns: new[] { "TenantProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_LastActivityUtc",
                table: "UserLoginHistory",
                column: "LastActivityUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_LogoutTimeUtc",
                table: "UserLoginHistory",
                column: "LogoutTimeUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginHistory_SessionKey",
                table: "UserLoginHistory",
                column: "SessionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueAssessmentBookings_ReservationId",
                table: "VenueAssessmentBookings",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueReservationAudits_ReservationId",
                table: "VenueReservationAudits",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueReservation_Venue_TimeSlot",
                table: "VenueReservations",
                columns: new[] { "VenueId", "StartTimeUtc", "EndTimeUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_VenueReservations_ExpiresOnUtc",
                table: "VenueReservations",
                column: "ExpiresOnUtc");

            migrationBuilder.CreateIndex(
                name: "IX_VenueReservations_FacilitatorId",
                table: "VenueReservations",
                column: "FacilitatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueReservations_Status",
                table: "VenueReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_Status",
                table: "Venues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "UX_Venue_Name_Campus",
                table: "Venues",
                columns: new[] { "Name", "Campus" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationCycle",
                schema: "FO");

            migrationBuilder.DropTable(
                name: "ApplicationEvent");

            migrationBuilder.DropTable(
                name: "ApplicationSubmissionQueue",
                schema: "FO");

            migrationBuilder.DropTable(
                name: "AssessmentAttachments");

            migrationBuilder.DropTable(
                name: "AssessmentAttemptAnswers");

            migrationBuilder.DropTable(
                name: "AssessmentQuestionOptions");

            migrationBuilder.DropTable(
                name: "BackgroundJobQueueItem",
                schema: "FO");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "EmployeeContact");

            migrationBuilder.DropTable(
                name: "Enrollments",
                schema: "Academics");

            migrationBuilder.DropTable(
                name: "Evidence");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Finance");

            migrationBuilder.DropTable(
                name: "FinancialClearance",
                schema: "Finance");

            migrationBuilder.DropTable(
                name: "ForekBase");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "InAppNotification",
                schema: "Alerts");

            migrationBuilder.DropTable(
                name: "LessonAttendances");

            migrationBuilder.DropTable(
                name: "LessonPlan");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "Medicals");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "NotificationContentBlocks");

            migrationBuilder.DropTable(
                name: "OnlineApplication",
                schema: "FO");

            migrationBuilder.DropTable(
                name: "OnlineApplicationUser",
                schema: "FO");

            migrationBuilder.DropTable(
                name: "PayslipRequest");

            migrationBuilder.DropTable(
                name: "Placements");

            migrationBuilder.DropTable(
                name: "RejectionTBL");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ReportSubReport");

            migrationBuilder.DropTable(
                name: "ResourceRoleAudiences");

            migrationBuilder.DropTable(
                name: "ResourceUserAudiences");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "StorageSettings");

            migrationBuilder.DropTable(
                name: "StoredDocumentContents");

            migrationBuilder.DropTable(
                name: "StudentAttachments");

            migrationBuilder.DropTable(
                name: "TenantSubscriptions");

            migrationBuilder.DropTable(
                name: "Training");

            migrationBuilder.DropTable(
                name: "UserLoginHistory");

            migrationBuilder.DropTable(
                name: "VenueAssessmentBookings");

            migrationBuilder.DropTable(
                name: "VenueReservationAudits");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "WorkplaceModules");

            migrationBuilder.DropTable(
                name: "AssessmentAttempts");

            migrationBuilder.DropTable(
                name: "AssessmentQuestions");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "Academics");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "NotificationEvents");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "StoredDocuments");

            migrationBuilder.DropTable(
                name: "TenantProfiles");

            migrationBuilder.DropTable(
                name: "VenueReservations");

            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "ContactPerson");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "ResourceCategory");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
