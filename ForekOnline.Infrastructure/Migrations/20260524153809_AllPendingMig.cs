using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllPendingMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("50dcb10d-5d7c-4b75-ac26-a5104b1add43"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("69dfc4b2-dbe3-449b-996c-bee80c05870f"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("7d1057f7-aa52-4816-8cf6-e3c8cdf36037"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("fa30fff4-84ed-4f61-9991-8edc06b758a2"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationEvents",
            //    keyColumn: "Id",
            //    keyValue: new Guid("aea5c804-4fed-488a-9523-8364ff0ce4f8"));

            migrationBuilder.AlterColumn<string>(
                name: "StudyPermitNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "StreetAddressLine2",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StreetAddressLine1",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationSource",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "PlaceOfBirth",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PassportNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameOfSchool",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IDNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HighestGrade",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                schema: "Academics",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                schema: "Academics",
                table: "Students",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cellphone",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AdmissionCategory",
                schema: "Academics",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternativePhone",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deregistered",
                schema: "Academics",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeregistrationDate",
                schema: "Academics",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeregistrationReason",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Disability",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GuardianId",
                schema: "Academics",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "HasDisability",
                schema: "Academics",
                table: "Students",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlacementId",
                schema: "Academics",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Province",
                schema: "Academics",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                schema: "Academics",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "StudyPermitExpiry",
                schema: "Academics",
                table: "Students",
                type: "datetime2",
                nullable: true);

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

            //migrationBuilder.InsertData(
            //    table: "NotificationContentBlocks",
            //    columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
            //    values: new object[,]
            //    {
            //        { new Guid("48e32bad-0a95-4b2a-a3c5-44269a597415"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 },
            //        { new Guid("b4bc6621-512d-491a-9c03-ba0ab34dca73"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 },
            //        { new Guid("f8e32b2c-57d0-467a-b05a-4f52680d4c0d"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 }
            //    });

            //migrationBuilder.UpdateData(
            //    table: "NotificationEvents",
            //    keyColumn: "Id",
            //    keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
            //    columns: new[] { "EndUtc", "StartUtc" },
            //    values: new object[] { new DateTime(2026, 5, 31, 15, 38, 7, 907, DateTimeKind.Utc).AddTicks(1101), new DateTime(2026, 5, 23, 15, 38, 7, 907, DateTimeKind.Utc).AddTicks(1101) });

            //migrationBuilder.InsertData(
            //    table: "NotificationEvents",
            //    columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
            //    values: new object[] { new Guid("1e0a0108-f2e2-4d84-a49e-a5ff2985e724"), null, null, 1, new DateTime(2026, 5, 31, 17, 38, 7, 906, DateTimeKind.Local).AddTicks(9208), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 5, 23, 17, 38, 7, 906, DateTimeKind.Local).AddTicks(9174), "Forek Online Version 2 Highlights" });

            //migrationBuilder.InsertData(
            //    table: "NotificationContentBlocks",
            //    columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
            //    values: new object[] { new Guid("dbc3b243-8c21-40f6-99c7-9099f156ace8"), null, null, null, new Guid("1e0a0108-f2e2-4d84-a49e-a5ff2985e724"), 0, null, "A sleeker, faster platform...", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_WorkplaceModules_PlacementId",
                table: "WorkplaceModules",
                column: "PlacementId");

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
                name: "IX_EnrollmentHistory_StudentId",
                table: "EnrollmentHistory",
                column: "StudentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                schema: "Academics",
                table: "Students",
                column: "GuardianId",
                principalTable: "Guardians",
                principalColumn: "GuardianId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Placements_PlacementId",
                schema: "Academics",
                table: "Students",
                column: "PlacementId",
                principalTable: "Placements",
                principalColumn: "PlacementId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkplaceModules_Placements_PlacementId",
                table: "WorkplaceModules",
                column: "PlacementId",
                principalTable: "Placements",
                principalColumn: "PlacementId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Placements_PlacementId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkplaceModules_Placements_PlacementId",
                table: "WorkplaceModules");

            migrationBuilder.DropTable(
                name: "EnrollmentHistory");

            migrationBuilder.DropTable(
                name: "StudentDocument");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropIndex(
                name: "IX_WorkplaceModules_PlacementId",
                table: "WorkplaceModules");

            migrationBuilder.DropIndex(
                name: "IX_Students_GuardianId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_PlacementId",
                schema: "Academics",
                table: "Students");

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("48e32bad-0a95-4b2a-a3c5-44269a597415"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("b4bc6621-512d-491a-9c03-ba0ab34dca73"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("dbc3b243-8c21-40f6-99c7-9099f156ace8"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationContentBlocks",
            //    keyColumn: "Id",
            //    keyValue: new Guid("f8e32b2c-57d0-467a-b05a-4f52680d4c0d"));

            //migrationBuilder.DeleteData(
            //    table: "NotificationEvents",
            //    keyColumn: "Id",
            //    keyValue: new Guid("1e0a0108-f2e2-4d84-a49e-a5ff2985e724"));

            migrationBuilder.DropColumn(
                name: "AlternativePhone",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Deregistered",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeregistrationDate",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeregistrationReason",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Disability",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "HasDisability",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PlacementId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Province",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudyPermitExpiry",
                schema: "Academics",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "StudyPermitNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "StreetAddressLine2",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StreetAddressLine1",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationSource",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PlaceOfBirth",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PassportNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NameOfSchool",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MiddleName",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IDNumber",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HighestGrade",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                schema: "Academics",
                table: "Students",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Cellphone",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionCategory",
                schema: "Academics",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            //migrationBuilder.InsertData(
            //    table: "NotificationContentBlocks",
            //    columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
            //    values: new object[,]
            //    {
            //        { new Guid("50dcb10d-5d7c-4b75-ac26-a5104b1add43"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 },
            //        { new Guid("7d1057f7-aa52-4816-8cf6-e3c8cdf36037"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 },
            //        { new Guid("fa30fff4-84ed-4f61-9991-8edc06b758a2"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 }
            //    });

            //migrationBuilder.UpdateData(
            //    table: "NotificationEvents",
            //    keyColumn: "Id",
            //    keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
            //    columns: new[] { "EndUtc", "StartUtc" },
            //    values: new object[] { new DateTime(2026, 5, 24, 19, 31, 12, 38, DateTimeKind.Utc).AddTicks(5443), new DateTime(2026, 5, 16, 19, 31, 12, 38, DateTimeKind.Utc).AddTicks(5443) });

            //migrationBuilder.InsertData(
            //    table: "NotificationEvents",
            //    columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
            //    values: new object[] { new Guid("aea5c804-4fed-488a-9523-8364ff0ce4f8"), null, null, 1, new DateTime(2026, 5, 24, 21, 31, 12, 37, DateTimeKind.Local).AddTicks(9727), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 5, 16, 21, 31, 12, 37, DateTimeKind.Local).AddTicks(9693), "Forek Online Version 2 Highlights" });

            //migrationBuilder.InsertData(
            //    table: "NotificationContentBlocks",
            //    columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
            //    values: new object[] { new Guid("69dfc4b2-dbe3-449b-996c-bee80c05870f"), null, null, null, new Guid("aea5c804-4fed-488a-9523-8364ff0ce4f8"), 0, null, "A sleeker, faster platform...", 0 });
        }
    }
}
