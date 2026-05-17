using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WblPlacementEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DigitalSignature",
                table: "Placements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlacementAgreement",
                table: "Placements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkplaceMentorEmail",
                table: "Placements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkplaceMentorName",
                table: "Placements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkplaceMentorPhone",
                table: "Placements",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DigitalSignature",
                table: "Placements");

            migrationBuilder.DropColumn(
                name: "PlacementAgreement",
                table: "Placements");

            migrationBuilder.DropColumn(
                name: "WorkplaceMentorEmail",
                table: "Placements");

            migrationBuilder.DropColumn(
                name: "WorkplaceMentorName",
                table: "Placements");

            migrationBuilder.DropColumn(
                name: "WorkplaceMentorPhone",
                table: "Placements");
        }
    }
}
