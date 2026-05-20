using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newWeekly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("8c43c1e8-64db-4a41-b087-bfc97dae5c9d"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("9f94d0ed-c4ad-4f0e-9127-ab4a9d8b1b4a"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("b1c7d4a2-75fc-46cb-8b5b-386593cfd598"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("b6f4594c-dffa-4723-899d-01936da8de47"));

            migrationBuilder.DeleteData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("2ec20796-c29a-4ea5-b98f-c45d714f3193"));

            migrationBuilder.AddColumn<string>(
                name: "ActionItemAssignee",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActionItemDueDate",
                table: "Visits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionItems",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AttendanceObserved",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EngagementObserved",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ObservationNotes",
                table: "Visits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlacementId",
                table: "Visits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SafetyObserved",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SkillApplicationObserved",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorkplaceConditionsObserved",
                table: "Visits",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[,]
                {
                    { new Guid("50dcb10d-5d7c-4b75-ac26-a5104b1add43"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 },
                    { new Guid("7d1057f7-aa52-4816-8cf6-e3c8cdf36037"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 },
                    { new Guid("fa30fff4-84ed-4f61-9991-8edc06b758a2"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 }
                });

            migrationBuilder.UpdateData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                columns: new[] { "EndUtc", "StartUtc" },
                values: new object[] { new DateTime(2026, 5, 24, 19, 31, 12, 38, DateTimeKind.Utc).AddTicks(5443), new DateTime(2026, 5, 16, 19, 31, 12, 38, DateTimeKind.Utc).AddTicks(5443) });

            migrationBuilder.InsertData(
                table: "NotificationEvents",
                columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
                values: new object[] { new Guid("aea5c804-4fed-488a-9523-8364ff0ce4f8"), null, null, 1, new DateTime(2026, 5, 24, 21, 31, 12, 37, DateTimeKind.Local).AddTicks(9727), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 5, 16, 21, 31, 12, 37, DateTimeKind.Local).AddTicks(9693), "Forek Online Version 2 Highlights" });

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[] { new Guid("69dfc4b2-dbe3-449b-996c-bee80c05870f"), null, null, null, new Guid("aea5c804-4fed-488a-9523-8364ff0ce4f8"), 0, null, "A sleeker, faster platform...", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PlacementId",
                table: "Visits",
                column: "PlacementId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyTimesheets_Placement_Week",
                table: "WeeklyTimesheets",
                columns: new[] { "PlacementId", "WeekStartDate", "WeekEndDate" });

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
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Placements_PlacementId",
                table: "Visits");

            migrationBuilder.DropTable(
                name: "WeeklyTimesheets");

            migrationBuilder.DropIndex(
                name: "IX_Visits_PlacementId",
                table: "Visits");

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("50dcb10d-5d7c-4b75-ac26-a5104b1add43"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("69dfc4b2-dbe3-449b-996c-bee80c05870f"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("7d1057f7-aa52-4816-8cf6-e3c8cdf36037"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("fa30fff4-84ed-4f61-9991-8edc06b758a2"));

            migrationBuilder.DeleteData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("aea5c804-4fed-488a-9523-8364ff0ce4f8"));

            migrationBuilder.DropColumn(
                name: "ActionItemAssignee",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ActionItemDueDate",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ActionItems",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "AttendanceObserved",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "EngagementObserved",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ObservationNotes",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "PlacementId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SafetyObserved",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "SkillApplicationObserved",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "WorkplaceConditionsObserved",
                table: "Visits");

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[,]
                {
                    { new Guid("8c43c1e8-64db-4a41-b087-bfc97dae5c9d"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 },
                    { new Guid("9f94d0ed-c4ad-4f0e-9127-ab4a9d8b1b4a"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 },
                    { new Guid("b6f4594c-dffa-4723-899d-01936da8de47"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 }
                });

            migrationBuilder.UpdateData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                columns: new[] { "EndUtc", "StartUtc" },
                values: new object[] { new DateTime(2026, 5, 24, 11, 22, 40, 440, DateTimeKind.Utc).AddTicks(3768), new DateTime(2026, 5, 16, 11, 22, 40, 440, DateTimeKind.Utc).AddTicks(3768) });

            migrationBuilder.InsertData(
                table: "NotificationEvents",
                columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
                values: new object[] { new Guid("2ec20796-c29a-4ea5-b98f-c45d714f3193"), null, null, 1, new DateTime(2026, 5, 24, 13, 22, 40, 440, DateTimeKind.Local).AddTicks(2559), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 5, 16, 13, 22, 40, 440, DateTimeKind.Local).AddTicks(2537), "Forek Online Version 2 Highlights" });

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[] { new Guid("b1c7d4a2-75fc-46cb-8b5b-386593cfd598"), null, null, null, new Guid("2ec20796-c29a-4ea5-b98f-c45d714f3193"), 0, null, "A sleeker, faster platform...", 0 });
        }
    }
}
