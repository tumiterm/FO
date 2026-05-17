using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReportExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("5ae75bdc-f643-4888-8d0a-88263a73871c"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("9a6acc3b-f458-4727-88fd-0579a0eb2932"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("b57e16db-163d-4395-8549-b818d8fa74da"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("fb56e589-dbb8-4f9f-b70b-7551fbec4b74"));

            migrationBuilder.DeleteData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("d0478f0c-7e16-4061-b870-7f4b0dbbd143"));

            migrationBuilder.AddColumn<DateTime>(
                name: "IntendedPeriodEnd",
                table: "Reports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntendedPeriodLabel",
                table: "Reports",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IntendedPeriodStart",
                table: "Reports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLateSubmission",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LateAcknowledgedUtc",
                table: "Reports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[,]
                {
                    { new Guid("92147494-9832-416c-8d5b-efc8120257f9"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 },
                    { new Guid("c971c2c2-8e89-4d32-9df9-1751504452bc"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 },
                    { new Guid("db252a14-7df3-4f7a-b388-32d3c91cfed7"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 }
                });

            migrationBuilder.UpdateData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                columns: new[] { "EndUtc", "StartUtc" },
                values: new object[] { new DateTime(2026, 4, 25, 11, 27, 38, 344, DateTimeKind.Utc).AddTicks(4766), new DateTime(2026, 4, 17, 11, 27, 38, 344, DateTimeKind.Utc).AddTicks(4766) });

            migrationBuilder.InsertData(
                table: "NotificationEvents",
                columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
                values: new object[] { new Guid("94e14490-7db4-48b7-a320-181c061c4c01"), null, null, 1, new DateTime(2026, 4, 25, 13, 27, 38, 344, DateTimeKind.Local).AddTicks(3921), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 4, 17, 13, 27, 38, 344, DateTimeKind.Local).AddTicks(3904), "Forek Online Version 2 Highlights" });

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[] { new Guid("f39dd93b-56c5-46f2-8f10-cf1d438ead39"), null, null, null, new Guid("94e14490-7db4-48b7-a320-181c061c4c01"), 0, null, "A sleeker, faster platform...", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("92147494-9832-416c-8d5b-efc8120257f9"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("c971c2c2-8e89-4d32-9df9-1751504452bc"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("db252a14-7df3-4f7a-b388-32d3c91cfed7"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("f39dd93b-56c5-46f2-8f10-cf1d438ead39"));

            migrationBuilder.DeleteData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("94e14490-7db4-48b7-a320-181c061c4c01"));

            migrationBuilder.DropColumn(
                name: "IntendedPeriodEnd",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IntendedPeriodLabel",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IntendedPeriodStart",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsLateSubmission",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "LateAcknowledgedUtc",
                table: "Reports");

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[,]
                {
                    { new Guid("9a6acc3b-f458-4727-88fd-0579a0eb2932"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 },
                    { new Guid("b57e16db-163d-4395-8549-b818d8fa74da"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 },
                    { new Guid("fb56e589-dbb8-4f9f-b70b-7551fbec4b74"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 }
                });

            migrationBuilder.UpdateData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                columns: new[] { "EndUtc", "StartUtc" },
                values: new object[] { new DateTime(2026, 4, 19, 17, 15, 22, 466, DateTimeKind.Utc).AddTicks(7023), new DateTime(2026, 4, 11, 17, 15, 22, 466, DateTimeKind.Utc).AddTicks(7023) });

            migrationBuilder.InsertData(
                table: "NotificationEvents",
                columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
                values: new object[] { new Guid("d0478f0c-7e16-4061-b870-7f4b0dbbd143"), null, null, 1, new DateTime(2026, 4, 19, 19, 15, 22, 466, DateTimeKind.Local).AddTicks(4951), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 4, 11, 19, 15, 22, 466, DateTimeKind.Local).AddTicks(4932), "Forek Online Version 2 Highlights" });

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[] { new Guid("5ae75bdc-f643-4888-8d0a-88263a73871c"), null, null, null, new Guid("d0478f0c-7e16-4061-b870-7f4b0dbbd143"), 0, null, "A sleeker, faster platform...", 0 });
        }
    }
}
