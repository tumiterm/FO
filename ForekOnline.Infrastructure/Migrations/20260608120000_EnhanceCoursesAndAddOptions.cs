using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260608120000_EnhanceCoursesAndAddOptions")]
public partial class EnhanceCoursesAndAddOptions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "MinimumRequirementNotes", table: "Course", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
        migrationBuilder.AddColumn<int>(name: "DurationValue", table: "Course", type: "int", nullable: true);
        migrationBuilder.AddColumn<int>(name: "DurationType", table: "Course", type: "int", nullable: true);
        migrationBuilder.AddColumn<int>(name: "StudyMode", table: "Course", type: "int", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<int>(name: "DeliveryMethod", table: "Course", type: "int", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<bool>(name: "IsAccredited", table: "Course", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<string>(name: "AccreditationBody", table: "Course", type: "nvarchar(200)", maxLength: 200, nullable: true);
        migrationBuilder.AddColumn<string>(name: "AccreditationNumber", table: "Course", type: "nvarchar(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<bool>(name: "RequiresAptitudeTest", table: "Course", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "RequiresInterview", table: "Course", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<decimal>(name: "ApplicationFee", table: "Course", type: "decimal(18,2)", nullable: true);
        migrationBuilder.AddColumn<decimal>(name: "RegistrationFee", table: "Course", type: "decimal(18,2)", nullable: true);
        migrationBuilder.AddColumn<decimal>(name: "TuitionFee", table: "Course", type: "decimal(18,2)", nullable: true);
        migrationBuilder.AddColumn<int>(name: "MaximumStudents", table: "Course", type: "int", nullable: true);
        migrationBuilder.AddColumn<bool>(name: "HasCourseOptions", table: "Course", type: "bit", nullable: false, defaultValue: false);

        migrationBuilder.CreateTable(
            name: "CourseOptions",
            columns: table => new
            {
                CourseOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CourseIdFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OptionDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                OptionType = table.Column<int>(type: "int", nullable: false),
                TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CourseOptions", x => x.CourseOptionId);
                table.ForeignKey("FK_CourseOptions_Course_CourseIdFK", x => x.CourseIdFK, "Course", "CourseId", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CourseOptionFees",
            columns: table => new
            {
                CourseOptionFeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CourseOptionIdFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FeeDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ChargeType = table.Column<int>(type: "int", nullable: false),
                Days = table.Column<int>(type: "int", nullable: true),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CourseOptionFees", x => x.CourseOptionFeeId);
                table.ForeignKey("FK_CourseOptionFees_CourseOptions_CourseOptionIdFK", x => x.CourseOptionIdFK, "CourseOptions", "CourseOptionId", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_CourseOptions_CourseIdFK", table: "CourseOptions", column: "CourseIdFK");
        migrationBuilder.CreateIndex(name: "IX_CourseOptionFees_CourseOptionIdFK", table: "CourseOptionFees", column: "CourseOptionIdFK");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CourseOptionFees");
        migrationBuilder.DropTable(name: "CourseOptions");
        foreach (var column in new[] { "MinimumRequirementNotes", "DurationValue", "DurationType", "StudyMode", "DeliveryMethod", "IsAccredited", "AccreditationBody", "AccreditationNumber", "RequiresAptitudeTest", "RequiresInterview", "ApplicationFee", "RegistrationFee", "TuitionFee", "MaximumStudents", "HasCourseOptions" })
            migrationBuilder.DropColumn(name: column, table: "Course");
    }
}
