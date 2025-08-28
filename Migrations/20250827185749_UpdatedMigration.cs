using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rezk_Proj.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkType",
                table: "Jobs",
                newName: "WorkTypeId");

            migrationBuilder.RenameColumn(
                name: "Salary",
                table: "Jobs",
                newName: "MinSalary");

            migrationBuilder.AddColumn<double>(
                name: "MaxSalary",
                table: "Jobs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

      
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_StatusTypes_StatusId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_WorkTypeLabels_WorkTypeId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "StatusTypes");

            migrationBuilder.DropTable(
                name: "WorkTypeLabels");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_WorkTypeId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Applications_StatusId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "WorkTypeId",
                table: "Jobs",
                newName: "WorkType");

            migrationBuilder.RenameColumn(
                name: "MinSalary",
                table: "Jobs",
                newName: "Salary");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Applications",
                newName: "Status");
        }
    }
}
