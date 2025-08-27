using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rezk_Proj.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "MinSalary",
                table: "Jobs",
                newName: "Salary");
        }
    }
}
