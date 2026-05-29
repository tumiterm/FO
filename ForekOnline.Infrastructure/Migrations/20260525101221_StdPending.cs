using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StdPending : Migration
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
                name: "Student",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasDisability = table.Column<bool>(type: "bit", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyPermitNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyPermitExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternativePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdmissionCategory = table.Column<int>(type: "int", nullable: false),
                    RegistrationSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighestGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameOfSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Deregistered = table.Column<bool>(type: "bit", nullable: false),
                    DeregistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeregistrationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuardianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_Student_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "GuardianId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Student_Placements_PlacementId",
                        column: x => x.PlacementId,
                        principalTable: "Placements",
                        principalColumn: "PlacementId");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "Academics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasDisability = table.Column<bool>(type: "bit", nullable: true),
                    Disability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyPermitNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyPermitExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cellphone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternativePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdmissionCategory = table.Column<int>(type: "int", nullable: false),
                    RegistrationSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighestGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameOfSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Deregistered = table.Column<bool>(type: "bit", nullable: false),
                    DeregistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeregistrationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuardianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeregistered = table.Column<bool>(type: "bit", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Students_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "GuardianId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Placements_PlacementId",
                        column: x => x.PlacementId,
                        principalTable: "Placements",
                        principalColumn: "PlacementId");
                });

            migrationBuilder.CreateTable(
                name: "WeeklyTimesheets",
                columns: table => new
                {
                    WeeklyTimesheetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeekStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalHours = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    MondayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TuesdayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WednesdayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ThursdayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FridayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SaturdayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SundayHours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActivityDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkillsApplied = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LearningOutcomes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChallengesFaced = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvidenceFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkplaceMentorDecisionBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkplaceMentorDecisionOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkplaceMentorComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CampusMentorAcknowledgedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampusMentorAcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CampusMentorComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyTimesheets", x => x.WeeklyTimesheetId);
                    table.ForeignKey(
                        name: "FK_WeeklyTimesheets_Placements_PlacementId",
                        column: x => x.PlacementId,
                        principalTable: "Placements",
                        principalColumn: "PlacementId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_WorkplaceModules_Placements_PlacementId",
                        column: x => x.PlacementId,
                        principalTable: "Placements",
                        principalColumn: "PlacementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentHistory",
                columns: table => new
                {
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrollmentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentHistory", x => x.EnrollmentId);
                    table.ForeignKey(
                        name: "FK_EnrollmentHistory_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "StudentId",
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
                name: "StudentDocument",
                columns: table => new
                {
                    StudentDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StudentEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocument", x => x.StudentDocumentId);
                    table.ForeignKey(
                        name: "FK_StudentDocument_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentDocument_Students_StudentEntityId",
                        column: x => x.StudentEntityId,
                        principalSchema: "Academics",
                        principalTable: "Students",
                        principalColumn: "Id");
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
                name: "IX_EnrollmentHistory_StudentId",
                table: "EnrollmentHistory",
                column: "StudentId");

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
                name: "IX_Student_GuardianId",
                table: "Student",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_PlacementId",
                table: "Student",
                column: "PlacementId",
                unique: true,
                filter: "[PlacementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocument_StudentEntityId",
                table: "StudentDocument",
                column: "StudentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocument_StudentId",
                table: "StudentDocument",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId",
                schema: "Academics",
                table: "Students",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PlacementId",
                schema: "Academics",
                table: "Students",
                column: "PlacementId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PlacementId",
                table: "Visits",
                column: "PlacementId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTimesheets_Placement_Week",
                table: "WeeklyTimesheets",
                columns: new[] { "PlacementId", "WeekStartDate", "WeekEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkplaceModules_PlacementId",
                table: "WorkplaceModules",
                column: "PlacementId");
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
                name: "EnrollmentHistory");

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
                name: "StudentDocument");

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
                name: "WeeklyTimesheets");

            migrationBuilder.DropTable(
                name: "WorkplaceModules");

            migrationBuilder.DropTable(
                name: "AssessmentAttempts");

            migrationBuilder.DropTable(
                name: "AssessmentQuestions");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "NotificationEvents");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "StoredDocuments");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "Academics");

            migrationBuilder.DropTable(
                name: "TenantProfiles");

            migrationBuilder.DropTable(
                name: "VenueReservations");

            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "ResourceCategory");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "Placements");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "ContactPerson");
        }
    }
}
