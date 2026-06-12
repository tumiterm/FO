using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ForekOnline.Infrastructure.Data;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260612120000_AddTenantOnboardingAndIsolation")]
public sealed class AddTenantOnboardingAndIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "Slug", table: "TenantProfiles", type: "nvarchar(80)", maxLength: 80, nullable: false, defaultValue: "default");
        migrationBuilder.AddColumn<string>(name: "LegalName", table: "TenantProfiles", type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: "Forek ICT Services");
        migrationBuilder.AddColumn<bool>(name: "IsActive", table: "TenantProfiles", type: "bit", nullable: false, defaultValue: true);
        migrationBuilder.AddColumn<string>(name: "TimeZoneId", table: "TenantProfiles", type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "Africa/Johannesburg");
        migrationBuilder.AddColumn<string>(name: "Culture", table: "TenantProfiles", type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "en-ZA");
        migrationBuilder.AddColumn<string>(name: "BillingContactEmail", table: "TenantProfiles", type: "nvarchar(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<string>(name: "ExternalCustomerReference", table: "TenantProfiles", type: "nvarchar(100)", maxLength: 100, nullable: true);

        migrationBuilder.AddColumn<Guid>(name: "TenantId", table: "Users", type: "uniqueidentifier", nullable: false, defaultValue: Guid.Parse("00000000-0000-0000-0000-000000000001"));
        migrationBuilder.AddColumn<Guid>(name: "TenantId", schema: "Academics", table: "Students", type: "uniqueidentifier", nullable: false, defaultValue: Guid.Parse("00000000-0000-0000-0000-000000000001"));

        migrationBuilder.CreateTable(
            name: "TenantDomains",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                HostName = table.Column<string>(type: "nvarchar(253)", maxLength: 253, nullable: false),
                IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                IsVerified = table.Column<bool>(type: "bit", nullable: false),
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
                table.PrimaryKey("PK_TenantDomains", x => x.Id);
                table.ForeignKey("FK_TenantDomains_TenantProfiles_TenantProfileId", x => x.TenantProfileId, "TenantProfiles", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_TenantProfiles_Slug", table: "TenantProfiles", column: "Slug", unique: true);
        migrationBuilder.CreateIndex(name: "IX_TenantProfiles_Code", table: "TenantProfiles", column: "Code", unique: true, filter: "[Code] IS NOT NULL");
        migrationBuilder.CreateIndex(name: "IX_TenantDomains_HostName", table: "TenantDomains", column: "HostName", unique: true);
        migrationBuilder.CreateIndex(name: "IX_TenantDomains_TenantProfileId", table: "TenantDomains", column: "TenantProfileId");
        migrationBuilder.CreateIndex(name: "IX_Users_TenantId", table: "Users", column: "TenantId");
        migrationBuilder.CreateIndex(name: "IX_Students_TenantId", schema: "Academics", table: "Students", column: "TenantId");

        migrationBuilder.AddForeignKey(name: "FK_Users_TenantProfiles_TenantId", table: "Users", column: "TenantId", principalTable: "TenantProfiles", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.AddForeignKey(name: "FK_Students_TenantProfiles_TenantId", schema: "Academics", table: "Students", column: "TenantId", principalTable: "TenantProfiles", principalColumn: "Id", onDelete: ReferentialAction.Restrict);

        migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM TenantSubscriptions WHERE TenantProfileId = '00000000-0000-0000-0000-000000000001')
            BEGIN
                INSERT INTO TenantSubscriptions
                    (Id, TenantProfileId, PlanName, StartsOn, ExpiresOn, GracePeriodDays, Status, Currency, Name, DateCreated, DateModified, IsDeleted)
                VALUES
                    (NEWID(), '00000000-0000-0000-0000-000000000001', 'Legacy', '2020-01-01', '2099-12-31', 7, 0, 'ZAR', 'Legacy', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 0)
            END
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Users_TenantProfiles_TenantId", table: "Users");
        migrationBuilder.DropForeignKey(name: "FK_Students_TenantProfiles_TenantId", table: "Students", schema: "Academics");
        migrationBuilder.DropTable(name: "TenantDomains");
        migrationBuilder.DropIndex(name: "IX_Users_TenantId", table: "Users");
        migrationBuilder.DropIndex(name: "IX_Students_TenantId", table: "Students", schema: "Academics");
        migrationBuilder.DropIndex(name: "IX_TenantProfiles_Slug", table: "TenantProfiles");
        migrationBuilder.DropIndex(name: "IX_TenantProfiles_Code", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "TenantId", table: "Users");
        migrationBuilder.DropColumn(name: "TenantId", table: "Students", schema: "Academics");
        migrationBuilder.DropColumn(name: "Slug", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "LegalName", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "IsActive", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "TimeZoneId", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "Culture", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "BillingContactEmail", table: "TenantProfiles");
        migrationBuilder.DropColumn(name: "ExternalCustomerReference", table: "TenantProfiles");
    }
}
