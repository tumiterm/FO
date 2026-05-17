using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReportEnhance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[,]
                {
                    { new Guid("00813b5a-bb66-4141-a2a5-92b4d715e513"), null, null, "Unified visual system (Dark Red / Red / Orange)||Enhanced statistics & quick scan KPIs||Optimized markup & reduced layout shift||Modular section architecture for future widgets||Accessibility-focused contrast & semantics", new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, null, 2 },
                    { new Guid("04fc2c94-dc9f-429c-a668-7b2626744d89"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "A sleeker, faster platform with cohesive red → orange design language and improved UX consistency.", 0 },
                    { new Guid("3a48e480-5fc8-43e9-9b3c-dd3845d1a719"), null, null, null, new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"), 0, null, "<div class='alert alert-warning'><i class='fa fa-triangle-exclamation me-2'></i>Please report anomalies to ICT so iterative refinements can ship rapidly.</div>", 0 }
                });

            migrationBuilder.UpdateData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("59ebc1e4-31dc-4b87-91c8-39d49c298b64"),
                columns: new[] { "EndUtc", "StartUtc" },
                values: new object[] { new DateTime(2026, 4, 26, 14, 15, 55, 433, DateTimeKind.Utc).AddTicks(6638), new DateTime(2026, 4, 18, 14, 15, 55, 433, DateTimeKind.Utc).AddTicks(6638) });

            migrationBuilder.InsertData(
                table: "NotificationEvents",
                columns: new[] { "Id", "AudienceRole", "CarouselGroupKey", "DisplayOrder", "EndUtc", "HeaderGradientCss", "HeaderIconCss", "HeaderTextColor", "ImageUrl", "IsActive", "ModalSize", "StartUtc", "Title" },
                values: new object[] { new Guid("dceac202-5f9b-47ca-ae56-8b97b16918e7"), null, null, 1, new DateTime(2026, 4, 26, 16, 15, 55, 433, DateTimeKind.Local).AddTicks(5840), "var(--ap-grad)", "fa fa-rocket", "#fff", "/Images/dancing.jpg", true, 2, new DateTime(2026, 4, 18, 16, 15, 55, 433, DateTimeKind.Local).AddTicks(5826), "Forek Online Version 2 Highlights" });

            migrationBuilder.InsertData(
                table: "NotificationContentBlocks",
                columns: new[] { "Id", "AltText", "ImageUrl", "ListItems", "NotificationEventId", "Order", "TableJson", "Text", "Type" },
                values: new object[] { new Guid("0a4937a0-344b-4443-85ef-4069f6d4d819"), null, null, null, new Guid("dceac202-5f9b-47ca-ae56-8b97b16918e7"), 0, null, "A sleeker, faster platform...", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("00813b5a-bb66-4141-a2a5-92b4d715e513"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("04fc2c94-dc9f-429c-a668-7b2626744d89"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("0a4937a0-344b-4443-85ef-4069f6d4d819"));

            migrationBuilder.DeleteData(
                table: "NotificationContentBlocks",
                keyColumn: "Id",
                keyValue: new Guid("3a48e480-5fc8-43e9-9b3c-dd3845d1a719"));

            migrationBuilder.DeleteData(
                table: "NotificationEvents",
                keyColumn: "Id",
                keyValue: new Guid("dceac202-5f9b-47ca-ae56-8b97b16918e7"));

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
    }
}
