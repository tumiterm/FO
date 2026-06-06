using System;
using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260606120000_FixDirectEnrollmentDocuments")]
    public partial class FixDirectEnrollmentDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentDocument_Student_StudentId",
                table: "StudentDocument");

            migrationBuilder.DropIndex(
                name: "IX_StudentDocument_StudentId",
                table: "StudentDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.AlterColumn<Guid>(
                name: "GuardianId",
                schema: "Academics",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                schema: "Academics",
                table: "Students",
                column: "GuardianId",
                principalTable: "Guardians",
                principalColumn: "GuardianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                schema: "Academics",
                table: "Students");

            migrationBuilder.AlterColumn<Guid>(
                name: "GuardianId",
                schema: "Academics",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocument_StudentId",
                table: "StudentDocument",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDocument_Student_StudentId",
                table: "StudentDocument",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                schema: "Academics",
                table: "Students",
                column: "GuardianId",
                principalTable: "Guardians",
                principalColumn: "GuardianId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
