using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlacementTimesheetsAndIntegratedVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlacementId",
                table: "Visits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(name: "AttendanceObserved", table: "Visits", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(name: "EngagementObserved", table: "Visits", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(name: "WorkplaceConditionsObserved", table: "Visits", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(name: "SafetyObserved", table: "Visits", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(name: "SkillApplicationObserved", table: "Visits", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<string>(name: "ObservationNotes", table: "Visits", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<string>(name: "ActionItems", table: "Visits", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<DateTime>(name: "ActionItemDueDate", table: "Visits", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>(name: "ActionItemAssignee", table: "Visits", type: "nvarchar(max)", nullable: true);

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
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateIndex(name: "IX_Visits_PlacementId", table: "Visits", column: "PlacementId");
            migrationBuilder.CreateIndex(name: "IX_WeeklyTimesheets_Placement_Week", table: "WeeklyTimesheets", columns: new[] { "PlacementId", "WeekStartDate", "WeekEndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Placements_PlacementId",
                table: "Visits",
                column: "PlacementId",
                principalTable: "Placements",
                principalColumn: "PlacementId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Visits_Placements_PlacementId", table: "Visits");
            migrationBuilder.DropTable(name: "WeeklyTimesheets");
            migrationBuilder.DropIndex(name: "IX_Visits_PlacementId", table: "Visits");
            migrationBuilder.DropColumn(name: "PlacementId", table: "Visits");
            migrationBuilder.DropColumn(name: "DurationMinutes", table: "Visits");
            migrationBuilder.DropColumn(name: "AttendanceObserved", table: "Visits");
            migrationBuilder.DropColumn(name: "EngagementObserved", table: "Visits");
            migrationBuilder.DropColumn(name: "WorkplaceConditionsObserved", table: "Visits");
            migrationBuilder.DropColumn(name: "SafetyObserved", table: "Visits");
            migrationBuilder.DropColumn(name: "SkillApplicationObserved", table: "Visits");
            migrationBuilder.DropColumn(name: "ObservationNotes", table: "Visits");
            migrationBuilder.DropColumn(name: "ActionItems", table: "Visits");
            migrationBuilder.DropColumn(name: "ActionItemDueDate", table: "Visits");
            migrationBuilder.DropColumn(name: "ActionItemAssignee", table: "Visits");
        }
    }
}
