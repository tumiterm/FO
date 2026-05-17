IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF SCHEMA_ID(N'FO') IS NULL EXEC(N'CREATE SCHEMA [FO];');
GO

IF SCHEMA_ID(N'Academics') IS NULL EXEC(N'CREATE SCHEMA [Academics];');
GO

CREATE TABLE [Address] (
    [AddressId] uniqueidentifier NOT NULL,
    [StreetName] nvarchar(max) NULL,
    [Line1] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [Province] int NULL,
    [PostalCode] nvarchar(max) NULL,
    [AssociativeId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY ([AddressId])
);
GO

CREATE TABLE [FO].[ApplicationCycle] (
    [Id] uniqueidentifier NOT NULL,
    [Description] nvarchar(max) NULL,
    [AcademicYear] int NOT NULL,
    [OpensAt] datetime2 NOT NULL,
    [Closes] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [ApplicationPeriod] nvarchar(450) NULL,
    [TurnaroundDays] int NOT NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [RowVersion] rowversion NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_ApplicationCycle] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ApplicationEvent] (
    [EventId] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [StartUtc] datetime2 NOT NULL,
    [EndUtc] datetime2 NULL,
    [AllDay] bit NOT NULL,
    [Category] nvarchar(max) NULL,
    [ColorHex] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [ModifiedOnUtc] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_ApplicationEvent] PRIMARY KEY ([EventId])
);
GO

CREATE TABLE [FO].[ApplicationSubmissionQueue] (
    [Id] uniqueidentifier NOT NULL,
    [OnlineApplicationUserId] uniqueidentifier NOT NULL,
    [CycleId] uniqueidentifier NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Attempts] int NOT NULL,
    [LockedUntilUtc] datetimeoffset NULL,
    [LockedBy] nvarchar(max) NULL,
    [DateCreatedUtc] datetimeoffset NOT NULL,
    [ProcessedUtc] datetimeoffset NULL,
    [AcknowledgementSentUtc] datetimeoffset NULL,
    [LastError] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_ApplicationSubmissionQueue] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AssessmentAttachments] (
    [AttachmentId] uniqueidentifier NOT NULL,
    [Document] nvarchar(max) NULL,
    [StudentNumber] nvarchar(max) NULL,
    [StudentId] uniqueidentifier NULL,
    [CourseId] uniqueidentifier NULL,
    [Module] int NULL,
    [ModuleId] uniqueidentifier NOT NULL,
    [Percentage] float NULL,
    [Attempts] int NOT NULL,
    [Type] int NOT NULL,
    [FileURL] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_AssessmentAttachments] PRIMARY KEY ([AttachmentId])
);
GO

CREATE TABLE [AssessmentAttempts] (
    [Id] uniqueidentifier NOT NULL,
    [AssessmentId] uniqueidentifier NOT NULL,
    [LearnerIdPass] nvarchar(max) NOT NULL,
    [StartedUtc] datetime2 NOT NULL,
    [SubmittedUtc] datetime2 NULL,
    [Status] int NOT NULL,
    [FinalScore] int NULL,
    [Percentage] float NULL,
    [FocusLossCount] int NOT NULL,
    [WasFullscreenViolated] bit NOT NULL,
    CONSTRAINT [PK_AssessmentAttempts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Assessments] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [TotalQuestions] int NOT NULL,
    [TimerMinutes] int NOT NULL,
    [IsPasswordProtected] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [AllowRetries] bit NOT NULL,
    [MaxRetryAttempts] int NULL,
    [ShuffleQuestions] bit NOT NULL,
    [ShuffleAnswers] bit NOT NULL,
    [ShowReviewAfter] bit NOT NULL,
    [EnforceFullscreen] bit NOT NULL,
    [MaxFocusLossAllowed] int NOT NULL,
    [IsActive] bit NOT NULL,
    [MaxScore] int NOT NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [ModifiedOnUtc] datetime2 NULL,
    [AssessmentType] int NOT NULL,
    [EnableMathRendering] bit NOT NULL,
    CONSTRAINT [PK_Assessments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FO].[BackgroundJobQueueItem] (
    [Id] uniqueidentifier NOT NULL,
    [Queue] nvarchar(450) NOT NULL,
    [JobType] nvarchar(450) NOT NULL,
    [PayloadJson] nvarchar(max) NOT NULL,
    [Status] nvarchar(450) NOT NULL,
    [Attempts] int NOT NULL,
    [LockedUntilUtc] datetimeoffset NULL,
    [LockedBy] nvarchar(max) NULL,
    [ProcessedUtc] datetimeoffset NULL,
    [LastError] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [RowVersion] rowversion NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_BackgroundJobQueueItem] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ContactPerson] (
    [ContactId] uniqueidentifier NOT NULL,
    [AssociativeId] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Cellphone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ContactPerson] PRIMARY KEY ([ContactId])
);
GO

CREATE TABLE [Course] (
    [CourseId] uniqueidentifier NOT NULL,
    [CourseName] nvarchar(max) NOT NULL,
    [Type] int NOT NULL,
    [NType] int NULL,
    [Credit] float NULL,
    [NQFLevel] int NULL,
    [IsEligibleForOnlineApplications] bit NOT NULL,
    [MinimumRequirement] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY ([CourseId])
);
GO

CREATE TABLE [Documents] (
    [DocumentId] uniqueidentifier NOT NULL,
    [Reference] nvarchar(max) NULL,
    [DocumentType] int NOT NULL,
    [RequestDate] datetime2 NOT NULL,
    [ReturnDate] datetime2 NULL,
    [Student] uniqueidentifier NULL,
    [RequestedBy] uniqueidentifier NOT NULL,
    [ModuleId] uniqueidentifier NULL,
    [CourseId] uniqueidentifier NULL,
    [Department] nvarchar(max) NOT NULL,
    [Designation] int NOT NULL,
    [Quantity] int NOT NULL,
    [RequestPurpose] nvarchar(max) NOT NULL,
    [ApprovedBy] uniqueidentifier NULL,
    [IsEmailIssued] bit NOT NULL,
    [IsHardCopyIssued] bit NOT NULL,
    [DocumentUpload] nvarchar(max) NULL,
    [SelectedStudentIDs] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY ([DocumentId])
);
GO

CREATE TABLE [EmployeeContact] (
    [EmployeeId] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Department] int NOT NULL,
    [Position] nvarchar(max) NOT NULL,
    [ProfileImageUrl] nvarchar(max) NULL,
    [IsCellNumberVisible] bit NOT NULL,
    CONSTRAINT [PK_EmployeeContact] PRIMARY KEY ([EmployeeId])
);
GO

CREATE TABLE [Evidence] (
    [EvidenceId] uniqueidentifier NOT NULL,
    [Module] int NOT NULL,
    [StudentNumber] nvarchar(max) NULL,
    [StudentId] uniqueidentifier NULL,
    [Photo] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Evidence] PRIMARY KEY ([EvidenceId])
);
GO

CREATE TABLE [Files] (
    [FileId] uniqueidentifier NOT NULL,
    [Type] int NOT NULL,
    [Phase] int NULL,
    [UserId] uniqueidentifier NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [Attachment] nvarchar(max) NULL,
    [BlobFileURL] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY ([FileId])
);
GO

CREATE TABLE [Finance] (
    [Id] uniqueidentifier NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [StudentNumber] nvarchar(max) NOT NULL,
    [File] nvarchar(max) NULL,
    [StatementName] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Finance] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ForekBase] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(100) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [PublicationDate] datetime2 NOT NULL,
    [Author] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ForekBase] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LessonAttendances] (
    [Id] uniqueidentifier NOT NULL,
    [LessonId] uniqueidentifier NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [JoinedUtc] datetime2 NULL,
    [LeftUtc] datetime2 NULL,
    [DurationSeconds] int NOT NULL,
    [LastEventUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_LessonAttendances] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LessonPlan] (
    [LessonPlanId] uniqueidentifier NOT NULL,
    [IdPass] nvarchar(max) NOT NULL,
    [Reference] nvarchar(max) NULL,
    [Course] uniqueidentifier NOT NULL,
    [Module] uniqueidentifier NOT NULL,
    [Phase] int NULL,
    [Funder] int NOT NULL,
    [Approval] int NOT NULL,
    [IsApprovedBy] nvarchar(max) NULL,
    [Reason] nvarchar(max) NULL,
    [Document] nvarchar(max) NULL,
    [UploadUrl] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_LessonPlan] PRIMARY KEY ([LessonPlanId])
);
GO

CREATE TABLE [Lessons] (
    [Id] uniqueidentifier NOT NULL,
    [RoomName] nvarchar(80) NOT NULL,
    [Topic] nvarchar(120) NOT NULL,
    [StartUtc] datetime2 NOT NULL,
    [EndUtc] datetime2 NOT NULL,
    [JoinUrl] nvarchar(400) NOT NULL,
    [Password] nvarchar(50) NULL,
    [CreatedBy] nvarchar(120) NOT NULL,
    [CreatedOnUtc] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Licenses] (
    [LicenseId] uniqueidentifier NOT NULL,
    [Title] int NOT NULL,
    [CourseKey] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [IDNumber] nvarchar(max) NULL,
    [DateOfIssue] datetime2 NOT NULL,
    [DateOfExpiry] datetime2 NOT NULL,
    [FileUpload] nvarchar(max) NULL,
    [ClientType] int NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [Frequency] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Licenses] PRIMARY KEY ([LicenseId])
);
GO

CREATE TABLE [Material] (
    [MaterialId] uniqueidentifier NOT NULL,
    [Document] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [Message] nvarchar(max) NULL,
    [DueDate] datetime2 NULL,
    [Trade] int NOT NULL,
    [Module] int NULL,
    [InstructorId] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Material] PRIMARY KEY ([MaterialId])
);
GO

CREATE TABLE [Medicals] (
    [MedicalId] uniqueidentifier NOT NULL,
    [MedicalName] nvarchar(max) NULL,
    [MemberNumber] nvarchar(max) NULL,
    [Telephone] nvarchar(max) NULL,
    [Disability] nvarchar(max) NULL,
    [ApplicationId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Medicals] PRIMARY KEY ([MedicalId])
);
GO

CREATE TABLE [NotificationEvents] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [HeaderIconCss] nvarchar(max) NULL,
    [HeaderGradientCss] nvarchar(max) NULL,
    [HeaderTextColor] nvarchar(max) NULL,
    [ModalSize] int NOT NULL,
    [ImageUrl] nvarchar(max) NULL,
    [StartUtc] datetime2 NOT NULL,
    [EndUtc] datetime2 NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [AudienceRole] nvarchar(max) NULL,
    [CarouselGroupKey] nvarchar(max) NULL,
    CONSTRAINT [PK_NotificationEvents] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FO].[OnlineApplication] (
    [Id] uniqueidentifier NOT NULL,
    [ApplicantId] uniqueidentifier NOT NULL,
    [CycleId] uniqueidentifier NOT NULL,
    [StudentNumber] nvarchar(20) NULL,
    [AcademicYear] int NOT NULL,
    [CourseTypeCode] nvarchar(2) NOT NULL,
    [FundingTypeCode] nvarchar(1) NOT NULL,
    [FirstChoiceCourseId] uniqueidentifier NULL,
    [SecondChoiceCourseId] uniqueidentifier NULL,
    [ThirdChoiceCourseId] uniqueidentifier NULL,
    [Status] nvarchar(30) NOT NULL,
    [SubmittedOnUtc] datetime2 NULL,
    [LastSavedOnUtc] datetime2 NOT NULL,
    [TatDays] int NULL,
    [HasExceededTurnaroundTime] bit NOT NULL,
    [LastTatEvaluatedOnUtc] datetime2 NULL,
    [OverdueNotifiedOnUtc] datetime2 NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_OnlineApplication] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FO].[OnlineApplicationUser] (
    [Id] uniqueidentifier NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [IdNumber] nvarchar(max) NULL,
    [PassportNumber] nvarchar(max) NULL,
    [Username] nvarchar(max) NOT NULL,
    [Cellphone] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [StudentNumber] nvarchar(max) NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [DateCreated] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [DateModified] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    [RowVersion] rowversion NOT NULL,
    CONSTRAINT [PK_OnlineApplicationUser] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PayslipRequest] (
    [Id] uniqueidentifier NOT NULL,
    [Document] nvarchar(max) NULL,
    [StartMonth] datetime2 NOT NULL,
    [EndMonth] datetime2 NULL,
    [PayslipUploaded] bit NOT NULL,
    [UploadDate] datetime2 NULL,
    [EmployeeId] uniqueidentifier NOT NULL,
    [DocumentType] int NOT NULL,
    [Reason] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_PayslipRequest] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Reports] (
    [ReportId] uniqueidentifier NOT NULL,
    [Reference] nvarchar(max) NULL,
    [IdPass] nvarchar(max) NULL,
    [ReportType] int NOT NULL,
    [Date] datetime2 NULL,
    [Module] nvarchar(max) NULL,
    [ActivityReport] nvarchar(max) NULL,
    [Challenges] nvarchar(max) NULL,
    [FacilitatorId] uniqueidentifier NULL,
    [Recommendation] nvarchar(max) NULL,
    [Urgency] int NULL,
    [Operation] int NOT NULL,
    [Document] nvarchar(max) NULL,
    [ReportURL] nvarchar(max) NULL,
    [IsRead] bit NOT NULL,
    [OpenCount] int NOT NULL,
    [LastOpened] datetime2 NULL,
    [HasExpired] bit NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [IsLiked] bit NULL,
    [LastDownloaded] datetime2 NULL,
    [DownloadCount] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY ([ReportId])
);
GO

CREATE TABLE [ReportSubReport] (
    [Id] uniqueidentifier NOT NULL,
    [ReportId] uniqueidentifier NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [Description] nvarchar(250) NULL,
    [FileId] nvarchar(max) NOT NULL,
    [LastDownloaded] datetime2 NULL,
    [DownloadCount] int NOT NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [RowVersion] varbinary(max) NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_ReportSubReport] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ResourceCategory] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_ResourceCategory] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Results] (
    [ReportId] uniqueidentifier NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [StudentNumber] nvarchar(max) NOT NULL,
    [Course] int NOT NULL,
    [ReportName] nvarchar(max) NOT NULL,
    [AttachReport] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Results] PRIMARY KEY ([ReportId])
);
GO

CREATE TABLE [StorageSettings] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [DocumentType] nvarchar(100) NULL,
    [ProviderType] nvarchar(100) NOT NULL,
    [ConnectionDetails] nvarchar(max) NULL,
    [MaxSizeMB] int NOT NULL,
    [EncryptAtRest] bit NOT NULL,
    [Compress] bit NOT NULL,
    [AllowedMimeTypesJson] nvarchar(max) NULL,
    [RetentionDays] int NOT NULL,
    [FallbackProviderType] nvarchar(100) NULL,
    [CustomMetadataJson] nvarchar(max) NULL,
    [IsDefault] bit NOT NULL,
    [EnablePresignedUrls] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_StorageSettings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StoredDocuments] (
    [Id] uniqueidentifier NOT NULL,
    [ProviderName] nvarchar(100) NOT NULL,
    [ProviderKey] nvarchar(512) NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [ContentType] nvarchar(255) NULL,
    [FileSizeBytes] bigint NOT NULL,
    [MetadataJson] nvarchar(max) NULL,
    [ExpiryDate] datetimeoffset NULL,
    [IsDeleted] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedOn] datetimeoffset NOT NULL,
    [ModifiedOn] datetimeoffset NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_StoredDocuments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudentAttachments] (
    [AttachmentId] uniqueidentifier NOT NULL,
    [DocumentName] int NOT NULL,
    [Document] nvarchar(max) NULL,
    [StudentNumber] nvarchar(max) NULL,
    [StudentId] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_StudentAttachments] PRIMARY KEY ([AttachmentId])
);
GO

CREATE TABLE [Academics].[Students] (
    [Id] uniqueidentifier NOT NULL,
    [StudentNumber] nvarchar(20) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [MiddleName] nvarchar(100) NULL,
    [LastName] nvarchar(100) NOT NULL,
    [IDNumber] nvarchar(13) NULL,
    [PassportNumber] nvarchar(20) NULL,
    [StudyPermitNumber] nvarchar(20) NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(10) NULL,
    [PlaceOfBirth] nvarchar(100) NULL,
    [Nationality] nvarchar(100) NULL,
    [Language] nvarchar(50) NULL,
    [AdmissionCategory] nvarchar(50) NULL,
    [StreetAddressLine1] nvarchar(250) NULL,
    [StreetAddressLine2] nvarchar(250) NULL,
    [Cellphone] nvarchar(15) NULL,
    [Email] nvarchar(256) NULL,
    [HighestGrade] nvarchar(50) NULL,
    [NameOfSchool] nvarchar(200) NULL,
    [AdmissionDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeregistered] bit NOT NULL,
    [RegistrationSource] nvarchar(30) NOT NULL,
    [OriginalApplicationId] uniqueidentifier NULL,
    [Code] nvarchar(50) NULL,
    [Name] nvarchar(250) NULL,
    [RowVersion] rowversion NOT NULL,
    [DateCreated] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [DateModified] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UserCreated] nvarchar(256) NULL,
    [UserModified] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Training] (
    [TrainingId] uniqueidentifier NOT NULL,
    [StudentNumber] nvarchar(max) NULL,
    [StudentId] uniqueidentifier NULL,
    [Document] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Training] PRIMARY KEY ([TrainingId])
);
GO

CREATE TABLE [UserLoginHistory] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [SessionKey] nvarchar(64) NOT NULL,
    [LoginTimeUtc] datetimeoffset NOT NULL,
    [LogoutTimeUtc] datetimeoffset NULL,
    [LastActivityUtc] datetimeoffset NULL,
    [IpAddress] nvarchar(45) NULL,
    [UserAgent] nvarchar(250) NULL,
    [DeviceType] nvarchar(100) NULL,
    [Browser] nvarchar(100) NULL,
    [IsCurrentSession] bit NOT NULL,
    [ForceLogoutPerformed] bit NULL,
    [LogoutReason] nvarchar(50) NULL,
    CONSTRAINT [PK_UserLoginHistory] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(25) NOT NULL,
    [LastName] nvarchar(25) NOT NULL,
    [Username] nvarchar(max) NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsEmailVerified] bit NOT NULL,
    [Department] int NULL,
    [ActivationCode] uniqueidentifier NULL,
    [ResetPasswordCode] nvarchar(max) NULL,
    [LastLoginDate] datetime2 NULL,
    [StudentNumber] nvarchar(max) NULL,
    [Cellphone] nvarchar(10) NULL,
    [IDPass] nvarchar(13) NULL,
    [Role] int NULL,
    [LastActivityDate] datetime2 NULL,
    [ProfileImage] nvarchar(max) NULL,
    [EmailSignatureLink] nvarchar(max) NULL,
    [IsHeadOfDepartment] bit NOT NULL,
    [ConfirmPassword] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Venues] (
    [Id] uniqueidentifier NOT NULL,
    [Campus] nvarchar(100) NOT NULL,
    [Departments] int NOT NULL,
    [MaxCapacity] int NOT NULL,
    [VenueType] int NOT NULL,
    [EquipmentChecklist] nvarchar(1000) NULL,
    [DefaultBookingRules] nvarchar(500) NULL,
    [PhotoUrl] nvarchar(500) NULL,
    [FloorPlanUrl] nvarchar(500) NULL,
    [Status] int NOT NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(150) NOT NULL,
    [RowVersion] varbinary(max) NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_Venues] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Visits] (
    [VisitId] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [HasReport] bit NOT NULL,
    [Report] nvarchar(max) NULL,
    [Date] datetime2 NOT NULL,
    [SelectedEmployeeIDs] nvarchar(max) NULL,
    [LearnerFeedback] nvarchar(max) NULL,
    [VisitPurpose] nvarchar(max) NULL,
    [Mentor] nvarchar(max) NULL,
    [VisitBy] uniqueidentifier NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Visits] PRIMARY KEY ([VisitId])
);
GO

CREATE TABLE [WorkplaceModules] (
    [LearnerWorkplaceModulesId] uniqueidentifier NOT NULL,
    [CourseId] uniqueidentifier NOT NULL,
    [ModuleId] uniqueidentifier NOT NULL,
    [Student] nvarchar(max) NULL,
    [PlacementId] uniqueidentifier NOT NULL,
    [Days] int NULL,
    [Progress] int NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_WorkplaceModules] PRIMARY KEY ([LearnerWorkplaceModulesId])
);
GO

CREATE TABLE [Applications] (
    [ApplicationId] uniqueidentifier NOT NULL,
    [ReferenceNumber] nvarchar(max) NULL,
    [Selection] int NOT NULL,
    [PassportNumber] nvarchar(max) NULL,
    [StudyPermitCategory] int NOT NULL,
    [IDNumber] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Cellphone] nvarchar(max) NOT NULL,
    [ApplicantAddressAddressId] uniqueidentifier NOT NULL,
    [ApplicantName] nvarchar(max) NOT NULL,
    [ApplicantSurname] nvarchar(max) NOT NULL,
    [ApplicantTitle] int NOT NULL,
    [IDPassFileUrl] nvarchar(max) NOT NULL,
    [HighestQualFileUrl] nvarchar(max) NOT NULL,
    [ResidenceFileUrl] nvarchar(max) NOT NULL,
    [Gender] int NOT NULL,
    [HighestQualification] int NOT NULL,
    [FunderType] int NOT NULL,
    [StatusReason] nvarchar(max) NULL,
    [IDPassDoc] nvarchar(max) NULL,
    [HighestQualDoc] nvarchar(max) NULL,
    [ResidenceDoc] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [PendingStatusReason] nvarchar(max) NULL,
    [PendingStatusMessage] nvarchar(250) NULL,
    [CourseId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY ([ApplicationId]),
    CONSTRAINT [FK_Applications_Address_ApplicantAddressAddressId] FOREIGN KEY ([ApplicantAddressAddressId]) REFERENCES [Address] ([AddressId]) ON DELETE CASCADE
);
GO

CREATE TABLE [AssessmentAttemptAnswers] (
    [Id] uniqueidentifier NOT NULL,
    [AssessmentAttemptId] uniqueidentifier NOT NULL,
    [AssessmentQuestionId] uniqueidentifier NOT NULL,
    [SelectedOptionId] uniqueidentifier NULL,
    [ShortAnswerValue] nvarchar(max) NULL,
    [IsCorrect] bit NULL,
    [MarksAwarded] int NULL,
    [DiagramAnnotationJson] nvarchar(max) NULL,
    [DiagramAnnotationSnapshotFileId] nvarchar(max) NULL,
    CONSTRAINT [PK_AssessmentAttemptAnswers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssessmentAttemptAnswers_AssessmentAttempts_AssessmentAttemptId] FOREIGN KEY ([AssessmentAttemptId]) REFERENCES [AssessmentAttempts] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AssessmentQuestions] (
    [Id] uniqueidentifier NOT NULL,
    [AssessmentId] uniqueidentifier NOT NULL,
    [QuestionType] int NOT NULL,
    [DisplayOrder] int NOT NULL,
    [Prompt] nvarchar(max) NOT NULL,
    [Explanation] nvarchar(max) NULL,
    [ImagePath] nvarchar(max) NULL,
    [EnableAnnotation] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [Marks] int NOT NULL,
    CONSTRAINT [PK_AssessmentQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssessmentQuestions_Assessments_AssessmentId] FOREIGN KEY ([AssessmentId]) REFERENCES [Assessments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Company] (
    [CompanyId] uniqueidentifier NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [AddressId] uniqueidentifier NOT NULL,
    [ContactId] uniqueidentifier NOT NULL,
    [Phone] nvarchar(max) NULL,
    [Speciality] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY ([CompanyId]),
    CONSTRAINT [FK_Company_Address_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Address] ([AddressId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Company_ContactPerson_ContactId] FOREIGN KEY ([ContactId]) REFERENCES [ContactPerson] ([ContactId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Module] (
    [ModuleId] uniqueidentifier NOT NULL,
    [ModuleName] nvarchar(max) NULL,
    [CourseIdFK] uniqueidentifier NOT NULL,
    [NQFLevel] int NULL,
    [Credit] float NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY ([ModuleId]),
    CONSTRAINT [FK_Module_Course_CourseIdFK] FOREIGN KEY ([CourseIdFK]) REFERENCES [Course] ([CourseId]) ON DELETE CASCADE
);
GO

CREATE TABLE [NotificationContentBlocks] (
    [Id] uniqueidentifier NOT NULL,
    [NotificationEventId] uniqueidentifier NOT NULL,
    [Type] int NOT NULL,
    [Text] nvarchar(max) NULL,
    [ListItems] nvarchar(max) NULL,
    [TableJson] nvarchar(max) NULL,
    [ImageUrl] nvarchar(max) NULL,
    [AltText] nvarchar(max) NULL,
    [Order] int NOT NULL,
    CONSTRAINT [PK_NotificationContentBlocks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_NotificationContentBlocks_NotificationEvents_NotificationEventId] FOREIGN KEY ([NotificationEventId]) REFERENCES [NotificationEvents] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Resource] (
    [Id] uniqueidentifier NOT NULL,
    [Type] int NOT NULL,
    [FileURL] nvarchar(max) NOT NULL,
    [ExternalUrl] nvarchar(max) NULL,
    [StoredFileId] nvarchar(max) NULL,
    [StoredFileProvider] nvarchar(max) NULL,
    [TagsCsv] nvarchar(max) NULL,
    [ViewsCount] int NOT NULL,
    [DownloadsCount] int NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [IsPublic] bit NOT NULL,
    [Code] nvarchar(max) NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [IsActive] bit NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [UserCreated] nvarchar(max) NOT NULL,
    [UserModified] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Resource] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Resource_ResourceCategory_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [ResourceCategory] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StoredDocumentContents] (
    [Id] uniqueidentifier NOT NULL,
    [Content] varbinary(max) NOT NULL,
    CONSTRAINT [PK_StoredDocumentContents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StoredDocumentContents_StoredDocuments_Id] FOREIGN KEY ([Id]) REFERENCES [StoredDocuments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Academics].[Enrollments] (
    [Id] uniqueidentifier NOT NULL,
    [StudentId] uniqueidentifier NOT NULL,
    [CourseId] uniqueidentifier NOT NULL,
    [CourseTitle] nvarchar(200) NULL,
    [CourseType] nvarchar(50) NULL,
    [EnrollmentStatus] nvarchar(30) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [DateCompleted] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [Code] nvarchar(50) NULL,
    [Name] nvarchar(250) NULL,
    [RowVersion] rowversion NOT NULL,
    [DateCreated] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [DateModified] datetimeoffset NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
    [UserCreated] nvarchar(256) NULL,
    [UserModified] nvarchar(256) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [VenueReservations] (
    [Id] uniqueidentifier NOT NULL,
    [VenueId] uniqueidentifier NOT NULL,
    [FacilitatorId] uniqueidentifier NOT NULL,
    [FacilitatorName] nvarchar(120) NOT NULL,
    [Campus] nvarchar(100) NOT NULL,
    [ExpectedStudents] int NOT NULL,
    [ReservedDate] datetime2 NOT NULL,
    [StartTimeUtc] datetime2 NOT NULL,
    [EndTimeUtc] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [RejectionReason] nvarchar(500) NULL,
    [ActionedByHodId] uniqueidentifier NULL,
    [ActionedByHodName] nvarchar(120) NULL,
    [ActionedOnUtc] datetime2 NULL,
    [ExpiresOnUtc] datetime2 NOT NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(150) NOT NULL,
    [RowVersion] rowversion NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_VenueReservations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VenueReservations_Venues_VenueId] FOREIGN KEY ([VenueId]) REFERENCES [Venues] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Guardians] (
    [GuardianId] uniqueidentifier NOT NULL,
    [ApplicationId] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Relationship] int NOT NULL,
    [Cellphone] nvarchar(max) NOT NULL,
    [IDDoc] nvarchar(max) NULL,
    CONSTRAINT [PK_Guardians] PRIMARY KEY ([GuardianId]),
    CONSTRAINT [FK_Guardians_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Applications] ([ApplicationId]) ON DELETE CASCADE
);
GO

CREATE TABLE [RejectionTBL] (
    [Id] uniqueidentifier NOT NULL,
    [ApplicationId] uniqueidentifier NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [AdditionalComments] nvarchar(max) NULL,
    [IsFinal] bit NOT NULL,
    [NextSteps] nvarchar(max) NULL,
    [FollowUpDate] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_RejectionTBL] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RejectionTBL_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Applications] ([ApplicationId]) ON DELETE CASCADE
);
GO

CREATE TABLE [AssessmentQuestionOptions] (
    [Id] uniqueidentifier NOT NULL,
    [AssessmentQuestionId] uniqueidentifier NOT NULL,
    [Text] nvarchar(max) NOT NULL,
    [IsCorrect] bit NOT NULL,
    [OrderIndex] int NOT NULL,
    CONSTRAINT [PK_AssessmentQuestionOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssessmentQuestionOptions_AssessmentQuestions_AssessmentQuestionId] FOREIGN KEY ([AssessmentQuestionId]) REFERENCES [AssessmentQuestions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Placements] (
    [PlacementId] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [Student] nvarchar(max) NOT NULL,
    [PlacedBy] uniqueidentifier NOT NULL,
    [Status] int NOT NULL,
    [StartDate] datetime2 NULL,
    [EndDate] datetime2 NULL,
    [Module] uniqueidentifier NULL,
    [CourseId] uniqueidentifier NULL,
    [Duration] int NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedOn] nvarchar(max) NULL,
    [ModifiedOn] nvarchar(max) NULL,
    [ModifiedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Placements] PRIMARY KEY ([PlacementId]),
    CONSTRAINT [FK_Placements_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([CompanyId]) ON DELETE CASCADE
);
GO

CREATE TABLE [ResourceRoleAudiences] (
    [Id] uniqueidentifier NOT NULL,
    [ResourceId] uniqueidentifier NOT NULL,
    [Role] int NOT NULL,
    CONSTRAINT [PK_ResourceRoleAudiences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResourceRoleAudiences_Resource_ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [Resource] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ResourceUserAudiences] (
    [Id] uniqueidentifier NOT NULL,
    [ResourceId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ResourceUserAudiences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResourceUserAudiences_Resource_ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [Resource] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ResourceUserAudiences_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VenueAssessmentBookings] (
    [Id] uniqueidentifier NOT NULL,
    [ReservationId] uniqueidentifier NOT NULL,
    [CourseId] uniqueidentifier NOT NULL,
    [ModuleId] uniqueidentifier NOT NULL,
    [AssessmentName] nvarchar(200) NOT NULL,
    [AssessmentType] int NULL,
    [Instructions] nvarchar(2000) NULL,
    [DurationMinutes] int NULL,
    [StudentList] nvarchar(max) NOT NULL,
    [EmailsSent] bit NOT NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(150) NOT NULL,
    [RowVersion] varbinary(max) NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_VenueAssessmentBookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VenueAssessmentBookings_VenueReservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [VenueReservations] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VenueReservationAudits] (
    [Id] uniqueidentifier NOT NULL,
    [ReservationId] uniqueidentifier NOT NULL,
    [Action] int NOT NULL,
    [Remarks] nvarchar(500) NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(150) NOT NULL,
    [RowVersion] varbinary(max) NOT NULL,
    [DateCreated] datetimeoffset NOT NULL,
    [DateModified] datetimeoffset NOT NULL,
    [UserCreated] nvarchar(max) NULL,
    [UserModified] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL,
    [DateDeleted] datetimeoffset NULL,
    CONSTRAINT [PK_VenueReservationAudits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VenueReservationAudits_VenueReservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [VenueReservations] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AudienceRole', N'CarouselGroupKey', N'DisplayOrder', N'EndUtc', N'HeaderGradientCss', N'HeaderIconCss', N'HeaderTextColor', N'ImageUrl', N'IsActive', N'ModalSize', N'StartUtc', N'Title') AND [object_id] = OBJECT_ID(N'[NotificationEvents]'))
    SET IDENTITY_INSERT [NotificationEvents] ON;
INSERT INTO [NotificationEvents] ([Id], [AudienceRole], [CarouselGroupKey], [DisplayOrder], [EndUtc], [HeaderGradientCss], [HeaderIconCss], [HeaderTextColor], [ImageUrl], [IsActive], [ModalSize], [StartUtc], [Title])
VALUES ('59ebc1e4-31dc-4b87-91c8-39d49c298b64', NULL, NULL, 1, '2026-04-03T20:26:07.4112587Z', N'var(--ap-grad)', N'fa fa-rocket', N'#fff', N'/Images/dancing.jpg', CAST(1 AS bit), 2, '2026-03-26T20:26:07.4112587Z', N'Forek Online Version 2 Highlights'),
('5e0cafae-7081-40fb-97ed-8fb35f3ded75', NULL, NULL, 1, '2026-04-03T22:26:07.4111540+02:00', N'var(--ap-grad)', N'fa fa-rocket', N'#fff', N'/Images/dancing.jpg', CAST(1 AS bit), 2, '2026-03-26T22:26:07.4111524+02:00', N'Forek Online Version 2 Highlights');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AudienceRole', N'CarouselGroupKey', N'DisplayOrder', N'EndUtc', N'HeaderGradientCss', N'HeaderIconCss', N'HeaderTextColor', N'ImageUrl', N'IsActive', N'ModalSize', N'StartUtc', N'Title') AND [object_id] = OBJECT_ID(N'[NotificationEvents]'))
    SET IDENTITY_INSERT [NotificationEvents] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AltText', N'ImageUrl', N'ListItems', N'NotificationEventId', N'Order', N'TableJson', N'Text', N'Type') AND [object_id] = OBJECT_ID(N'[NotificationContentBlocks]'))
    SET IDENTITY_INSERT [NotificationContentBlocks] ON;
INSERT INTO [NotificationContentBlocks] ([Id], [AltText], [ImageUrl], [ListItems], [NotificationEventId], [Order], [TableJson], [Text], [Type])
VALUES ('6a60e31c-34ea-4e65-9f37-052024c93512', NULL, NULL, NULL, '5e0cafae-7081-40fb-97ed-8fb35f3ded75', 0, NULL, N'A sleeker, faster platform...', 0),
('82bace3e-b10b-40ce-b1e1-8cb08bc70d0a', NULL, NULL, NULL, '59ebc1e4-31dc-4b87-91c8-39d49c298b64', 0, NULL, N'A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.', 0),
('d6f728ab-95d8-4105-a1bb-aa6ac4002686', NULL, NULL, NULL, '59ebc1e4-31dc-4b87-91c8-39d49c298b64', 0, NULL, N'<div class=''alert alert-warning''><i class=''fa fa-triangle-exclamation me-2''></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>', 0),
('e8290830-30d4-49c6-b0b9-c65e6b7b9f9a', NULL, NULL, N'Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics', '59ebc1e4-31dc-4b87-91c8-39d49c298b64', 0, NULL, NULL, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AltText', N'ImageUrl', N'ListItems', N'NotificationEventId', N'Order', N'TableJson', N'Text', N'Type') AND [object_id] = OBJECT_ID(N'[NotificationContentBlocks]'))
    SET IDENTITY_INSERT [NotificationContentBlocks] OFF;
GO

CREATE INDEX [IX_ApplicationCycle_AcademicYear] ON [FO].[ApplicationCycle] ([AcademicYear]);
GO

CREATE INDEX [IX_ApplicationCycle_Year_Period] ON [FO].[ApplicationCycle] ([AcademicYear], [ApplicationPeriod]);
GO

CREATE UNIQUE INDEX [UX_ApplicationCycle_Year_Period_Active] ON [FO].[ApplicationCycle] ([AcademicYear], [ApplicationPeriod], [IsActive]) WHERE [IsActive] = 1 AND [IsDeleted] = 0;
GO

CREATE INDEX [IX_Applications_ApplicantAddressAddressId] ON [Applications] ([ApplicantAddressAddressId]);
GO

CREATE INDEX [IX_AssessmentAttemptAnswers_AssessmentAttemptId] ON [AssessmentAttemptAnswers] ([AssessmentAttemptId]);
GO

CREATE INDEX [IX_AssessmentQuestionOptions_AssessmentQuestionId] ON [AssessmentQuestionOptions] ([AssessmentQuestionId]);
GO

CREATE INDEX [IX_AssessmentQuestions_AssessmentId] ON [AssessmentQuestions] ([AssessmentId]);
GO

CREATE INDEX [IX_BackgroundJobQueueItem_DateCreated] ON [FO].[BackgroundJobQueueItem] ([DateCreated]);
GO

CREATE INDEX [IX_BackgroundJobQueueItem_JobType] ON [FO].[BackgroundJobQueueItem] ([JobType]);
GO

CREATE INDEX [IX_BackgroundJobQueueItem_Q_Status_Lock_Created] ON [FO].[BackgroundJobQueueItem] ([Queue], [Status], [LockedUntilUtc], [DateCreated]);
GO

CREATE INDEX [IX_BackgroundJobQueueItem_Queue_Status_Lock] ON [FO].[BackgroundJobQueueItem] ([Queue], [Status], [LockedUntilUtc]);
GO

CREATE INDEX [IX_Company_AddressId] ON [Company] ([AddressId]);
GO

CREATE INDEX [IX_Company_ContactId] ON [Company] ([ContactId]);
GO

CREATE INDEX [IX_Enrollment_IsActive] ON [Academics].[Enrollments] ([IsActive]);
GO

CREATE UNIQUE INDEX [UX_Enrollment_Student_Course] ON [Academics].[Enrollments] ([StudentId], [CourseId]);
GO

CREATE UNIQUE INDEX [IX_Guardians_ApplicationId] ON [Guardians] ([ApplicationId]);
GO

CREATE UNIQUE INDEX [IX_LessonAttendances_LessonId_StudentId] ON [LessonAttendances] ([LessonId], [StudentId]);
GO

CREATE INDEX [IX_Lessons_RoomName] ON [Lessons] ([RoomName]);
GO

CREATE INDEX [IX_Module_CourseIdFK] ON [Module] ([CourseIdFK]);
GO

CREATE INDEX [IX_NotificationContentBlocks_NotificationEventId] ON [NotificationContentBlocks] ([NotificationEventId]);
GO

CREATE INDEX [IX_Placements_CompanyId] ON [Placements] ([CompanyId]);
GO

CREATE INDEX [IX_RejectionTBL_ApplicationId] ON [RejectionTBL] ([ApplicationId]);
GO

CREATE INDEX [IX_ReportSubReport_ReportId] ON [ReportSubReport] ([ReportId]);
GO

CREATE INDEX [IX_Resource_CategoryId] ON [Resource] ([CategoryId]);
GO

CREATE UNIQUE INDEX [IX_ResourceRoleAudiences_ResourceId_Role] ON [ResourceRoleAudiences] ([ResourceId], [Role]);
GO

CREATE UNIQUE INDEX [IX_ResourceUserAudiences_ResourceId_UserId] ON [ResourceUserAudiences] ([ResourceId], [UserId]);
GO

CREATE INDEX [IX_ResourceUserAudiences_UserId] ON [ResourceUserAudiences] ([UserId]);
GO

CREATE INDEX [IX_Student_Email] ON [Academics].[Students] ([Email]);
GO

CREATE INDEX [IX_Student_IDNumber] ON [Academics].[Students] ([IDNumber]);
GO

CREATE INDEX [IX_Student_PassportNumber] ON [Academics].[Students] ([PassportNumber]);
GO

CREATE UNIQUE INDEX [UX_Student_StudentNumber] ON [Academics].[Students] ([StudentNumber]);
GO

CREATE INDEX [IX_UserLoginHistory_LastActivityUtc] ON [UserLoginHistory] ([LastActivityUtc]);
GO

CREATE INDEX [IX_UserLoginHistory_LogoutTimeUtc] ON [UserLoginHistory] ([LogoutTimeUtc]);
GO

CREATE UNIQUE INDEX [IX_UserLoginHistory_SessionKey] ON [UserLoginHistory] ([SessionKey]);
GO

CREATE UNIQUE INDEX [IX_VenueAssessmentBookings_ReservationId] ON [VenueAssessmentBookings] ([ReservationId]);
GO

CREATE INDEX [IX_VenueReservationAudits_ReservationId] ON [VenueReservationAudits] ([ReservationId]);
GO

CREATE INDEX [IX_VenueReservation_Venue_TimeSlot] ON [VenueReservations] ([VenueId], [StartTimeUtc], [EndTimeUtc]);
GO

CREATE INDEX [IX_VenueReservations_ExpiresOnUtc] ON [VenueReservations] ([ExpiresOnUtc]);
GO

CREATE INDEX [IX_VenueReservations_FacilitatorId] ON [VenueReservations] ([FacilitatorId]);
GO

CREATE INDEX [IX_VenueReservations_Status] ON [VenueReservations] ([Status]);
GO

CREATE INDEX [IX_Venues_Status] ON [Venues] ([Status]);
GO

CREATE UNIQUE INDEX [UX_Venue_Name_Campus] ON [Venues] ([Name], [Campus]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260327202608_InitialCreate', N'8.0.1');
GO

COMMIT;
GO

