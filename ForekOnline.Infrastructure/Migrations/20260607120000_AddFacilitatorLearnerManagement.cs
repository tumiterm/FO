using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260607120000_AddFacilitatorLearnerManagement")]
public partial class AddFacilitatorLearnerManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "Learning");

        migrationBuilder.Sql("""
CREATE TABLE [Learning].[FacilitatorStudentLinks] (
 [Id] uniqueidentifier NOT NULL, [FacilitatorId] uniqueidentifier NOT NULL, [StudentId] uniqueidentifier NOT NULL,
 [DateAdded] datetimeoffset NOT NULL, [AddedById] uniqueidentifier NOT NULL, [Status] int NOT NULL,
 [DateRemoved] datetimeoffset NULL, [RemovedById] uniqueidentifier NULL, [RemovalReason] nvarchar(250) NULL, [Notes] nvarchar(1000) NULL,
 [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL,
 [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL,
 [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_FacilitatorStudentLinks] PRIMARY KEY ([Id]),
 CONSTRAINT [FK_FacilitatorStudentLinks_Users_FacilitatorId] FOREIGN KEY ([FacilitatorId]) REFERENCES [Users] ([Id]),
 CONSTRAINT [FK_FacilitatorStudentLinks_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id])
);
CREATE INDEX [IX_FacilitatorStudentLinks_FacilitatorId_StudentId_Status] ON [Learning].[FacilitatorStudentLinks] ([FacilitatorId], [StudentId], [Status]);
CREATE INDEX [IX_FacilitatorStudentLinks_StudentId] ON [Learning].[FacilitatorStudentLinks] ([StudentId]);

CREATE TABLE [Learning].[LearningGroups] (
 [Id] uniqueidentifier NOT NULL, [FacilitatorId] uniqueidentifier NOT NULL, [GroupName] nvarchar(120) NOT NULL, [Color] nvarchar(7) NOT NULL,
 [Note] nvarchar(1500) NULL, [Status] int NOT NULL, [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL,
 [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL,
 [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_LearningGroups] PRIMARY KEY ([Id]), CONSTRAINT [FK_LearningGroups_Users_FacilitatorId] FOREIGN KEY ([FacilitatorId]) REFERENCES [Users] ([Id])
);
CREATE INDEX [IX_LearningGroups_FacilitatorId_Status] ON [Learning].[LearningGroups] ([FacilitatorId], [Status]);

CREATE TABLE [Learning].[LearningGroupStudents] (
 [Id] uniqueidentifier NOT NULL, [LearningGroupId] uniqueidentifier NOT NULL, [FacilitatorStudentLinkId] uniqueidentifier NOT NULL,
 [DateAdded] datetimeoffset NOT NULL, [AddedById] uniqueidentifier NOT NULL, [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL,
 [RowVersion] rowversion NOT NULL, [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL, [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_LearningGroupStudents] PRIMARY KEY ([Id]),
 CONSTRAINT [FK_LearningGroupStudents_LearningGroups_LearningGroupId] FOREIGN KEY ([LearningGroupId]) REFERENCES [Learning].[LearningGroups] ([Id]) ON DELETE CASCADE,
 CONSTRAINT [FK_LearningGroupStudents_FacilitatorStudentLinks_FacilitatorStudentLinkId] FOREIGN KEY ([FacilitatorStudentLinkId]) REFERENCES [Learning].[FacilitatorStudentLinks] ([Id])
);
CREATE UNIQUE INDEX [IX_LearningGroupStudents_LearningGroupId_FacilitatorStudentLinkId] ON [Learning].[LearningGroupStudents] ([LearningGroupId], [FacilitatorStudentLinkId]) WHERE [IsDeleted] = 0;
CREATE INDEX [IX_LearningGroupStudents_FacilitatorStudentLinkId] ON [Learning].[LearningGroupStudents] ([FacilitatorStudentLinkId]);

CREATE TABLE [Learning].[AttendanceSessions] (
 [Id] uniqueidentifier NOT NULL, [FacilitatorId] uniqueidentifier NOT NULL, [LearningGroupId] uniqueidentifier NOT NULL, [AttendanceDate] datetime2 NOT NULL,
 [StartTime] time NULL, [EndTime] time NULL, [Topic] nvarchar(250) NULL, [Note] nvarchar(1500) NULL, [Status] int NOT NULL, [SubmittedUtc] datetimeoffset NULL,
 [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL, [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL, [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_AttendanceSessions] PRIMARY KEY ([Id]), CONSTRAINT [FK_AttendanceSessions_Users_FacilitatorId] FOREIGN KEY ([FacilitatorId]) REFERENCES [Users] ([Id]),
 CONSTRAINT [FK_AttendanceSessions_LearningGroups_LearningGroupId] FOREIGN KEY ([LearningGroupId]) REFERENCES [Learning].[LearningGroups] ([Id])
);
CREATE INDEX [IX_AttendanceSessions_FacilitatorId_LearningGroupId_AttendanceDate] ON [Learning].[AttendanceSessions] ([FacilitatorId], [LearningGroupId], [AttendanceDate]);
CREATE INDEX [IX_AttendanceSessions_LearningGroupId] ON [Learning].[AttendanceSessions] ([LearningGroupId]);

CREATE TABLE [Learning].[AttendanceRecords] (
 [Id] uniqueidentifier NOT NULL, [AttendanceSessionId] uniqueidentifier NOT NULL, [StudentId] uniqueidentifier NOT NULL, [AttendanceStatus] int NOT NULL,
 [Comment] nvarchar(1000) NULL, [MarkedById] uniqueidentifier NOT NULL, [MarkedUtc] datetimeoffset NOT NULL, [UpdatedById] uniqueidentifier NULL, [UpdatedUtc] datetimeoffset NULL,
 [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL, [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL, [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_AttendanceRecords] PRIMARY KEY ([Id]), CONSTRAINT [FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId] FOREIGN KEY ([AttendanceSessionId]) REFERENCES [Learning].[AttendanceSessions] ([Id]) ON DELETE CASCADE,
 CONSTRAINT [FK_AttendanceRecords_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id])
);
CREATE UNIQUE INDEX [IX_AttendanceRecords_AttendanceSessionId_StudentId] ON [Learning].[AttendanceRecords] ([AttendanceSessionId], [StudentId]);
CREATE INDEX [IX_AttendanceRecords_StudentId] ON [Learning].[AttendanceRecords] ([StudentId]);

CREATE TABLE [Learning].[AttendanceRecordAudits] (
 [Id] uniqueidentifier NOT NULL, [AttendanceRecordId] uniqueidentifier NOT NULL, [OriginalStatus] int NOT NULL, [NewStatus] int NOT NULL,
 [OriginalComment] nvarchar(1000) NULL, [NewComment] nvarchar(1000) NULL, [EditedById] uniqueidentifier NOT NULL, [EditedUtc] datetimeoffset NOT NULL, [Reason] nvarchar(500) NULL,
 [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL, [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL, [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_AttendanceRecordAudits] PRIMARY KEY ([Id]), CONSTRAINT [FK_AttendanceRecordAudits_AttendanceRecords_AttendanceRecordId] FOREIGN KEY ([AttendanceRecordId]) REFERENCES [Learning].[AttendanceRecords] ([Id]) ON DELETE CASCADE
);
CREATE INDEX [IX_AttendanceRecordAudits_AttendanceRecordId] ON [Learning].[AttendanceRecordAudits] ([AttendanceRecordId]);

CREATE TABLE [Learning].[LearnerMessageLogs] (
 [Id] uniqueidentifier NOT NULL, [FacilitatorId] uniqueidentifier NOT NULL, [LearningGroupId] uniqueidentifier NULL, [StudentId] uniqueidentifier NOT NULL,
 [Channel] int NOT NULL, [Status] int NOT NULL, [Message] nvarchar(500) NOT NULL, [Destination] nvarchar(100) NULL, [ProviderResponse] nvarchar(1000) NULL,
 [QueuedUtc] datetimeoffset NOT NULL, [SentUtc] datetimeoffset NULL, [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL,
 [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL, [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_LearnerMessageLogs] PRIMARY KEY ([Id]), CONSTRAINT [FK_LearnerMessageLogs_Users_FacilitatorId] FOREIGN KEY ([FacilitatorId]) REFERENCES [Users] ([Id]),
 CONSTRAINT [FK_LearnerMessageLogs_LearningGroups_LearningGroupId] FOREIGN KEY ([LearningGroupId]) REFERENCES [Learning].[LearningGroups] ([Id]) ON DELETE SET NULL,
 CONSTRAINT [FK_LearnerMessageLogs_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Academics].[Students] ([Id])
);
CREATE INDEX [IX_LearnerMessageLogs_FacilitatorId_Channel_QueuedUtc] ON [Learning].[LearnerMessageLogs] ([FacilitatorId], [Channel], [QueuedUtc]);
CREATE INDEX [IX_LearnerMessageLogs_LearningGroupId] ON [Learning].[LearnerMessageLogs] ([LearningGroupId]);
CREATE INDEX [IX_LearnerMessageLogs_StudentId] ON [Learning].[LearnerMessageLogs] ([StudentId]);

CREATE TABLE [Learning].[FacilitatorActivityAudits] (
 [Id] uniqueidentifier NOT NULL, [ActorUserId] uniqueidentifier NOT NULL, [EventType] nvarchar(100) NOT NULL, [EntityType] nvarchar(100) NOT NULL,
 [EntityId] uniqueidentifier NULL, [PreviousValue] nvarchar(2000) NULL, [NewValue] nvarchar(2000) NULL, [Notes] nvarchar(1000) NULL, [EventUtc] datetimeoffset NOT NULL,
 [Code] nvarchar(50) NULL, [Name] nvarchar(250) NULL, [RowVersion] rowversion NOT NULL, [DateCreated] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(),
 [DateModified] datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(), [UserCreated] nvarchar(256) NULL, [UserModified] nvarchar(256) NULL, [IsDeleted] bit NOT NULL, [DateDeleted] datetimeoffset NULL,
 CONSTRAINT [PK_FacilitatorActivityAudits] PRIMARY KEY ([Id]), CONSTRAINT [FK_FacilitatorActivityAudits_Users_ActorUserId] FOREIGN KEY ([ActorUserId]) REFERENCES [Users] ([Id])
);
CREATE INDEX [IX_FacilitatorActivityAudits_ActorUserId_EventUtc] ON [Learning].[FacilitatorActivityAudits] ([ActorUserId], [EventUtc]);
""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AttendanceRecordAudits", schema: "Learning");
        migrationBuilder.DropTable(name: "FacilitatorActivityAudits", schema: "Learning");
        migrationBuilder.DropTable(name: "LearnerMessageLogs", schema: "Learning");
        migrationBuilder.DropTable(name: "AttendanceRecords", schema: "Learning");
        migrationBuilder.DropTable(name: "LearningGroupStudents", schema: "Learning");
        migrationBuilder.DropTable(name: "AttendanceSessions", schema: "Learning");
        migrationBuilder.DropTable(name: "FacilitatorStudentLinks", schema: "Learning");
        migrationBuilder.DropTable(name: "LearningGroups", schema: "Learning");
    }
}
