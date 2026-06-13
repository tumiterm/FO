using ForekOnline.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForekOnline.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260613120000_AddRoleAccessGovernance")]
public sealed class AddRoleAccessGovernance : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "Security");
        migrationBuilder.CreateTable(
            name: "RoleAccessRequests", schema: "Security",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FromRole = table.Column<int>(type: "int", nullable: true),
                RequestedRole = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                RequestedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ReviewedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReviewNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                LastReminderUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReminderCount = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleAccessRequests", x => x.Id);
                table.ForeignKey("FK_RoleAccessRequests_Users_TargetUserId", x => x.TargetUserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_RoleAccessRequests_Users_RequestedByUserId", x => x.RequestedByUserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_RoleAccessRequests_Users_ReviewedByUserId", x => x.ReviewedByUserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "UserRoleHistory", schema: "Security",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FromRole = table.Column<int>(type: "int", nullable: true),
                ToRole = table.Column<int>(type: "int", nullable: true),
                ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ChangedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                RoleAccessRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedOn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoleHistory", x => x.Id);
                table.ForeignKey("FK_UserRoleHistory_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_UserRoleHistory_Users_ChangedByUserId", x => x.ChangedByUserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_UserRoleHistory_RoleAccessRequests_RoleAccessRequestId", x => x.RoleAccessRequestId, principalSchema: "Security", principalTable: "RoleAccessRequests", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex("IX_RoleAccessRequests_TenantId_Status_RequestedUtc", "RoleAccessRequests", new[] { "TenantId", "Status", "RequestedUtc" }, schema: "Security");
        migrationBuilder.CreateIndex("IX_RoleAccessRequests_TargetUserId_Status", "RoleAccessRequests", new[] { "TargetUserId", "Status" }, schema: "Security", unique: true, filter: "[Status] = 0");
        migrationBuilder.CreateIndex("IX_RoleAccessRequests_RequestedByUserId", "RoleAccessRequests", "RequestedByUserId", schema: "Security");
        migrationBuilder.CreateIndex("IX_RoleAccessRequests_ReviewedByUserId", "RoleAccessRequests", "ReviewedByUserId", schema: "Security");
        migrationBuilder.CreateIndex("IX_UserRoleHistory_UserId_ChangedUtc", "UserRoleHistory", new[] { "UserId", "ChangedUtc" }, schema: "Security");
        migrationBuilder.CreateIndex("IX_UserRoleHistory_ChangedByUserId", "UserRoleHistory", "ChangedByUserId", schema: "Security");
        migrationBuilder.CreateIndex("IX_UserRoleHistory_RoleAccessRequestId", "UserRoleHistory", "RoleAccessRequestId", schema: "Security");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserRoleHistory", schema: "Security");
        migrationBuilder.DropTable(name: "RoleAccessRequests", schema: "Security");
    }
}
