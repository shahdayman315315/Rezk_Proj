using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rezk_Proj.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Applicants_ApplicantId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications");

            //migrationBuilder.DropColumn(
            //    name: "Salary",
            //    table: "Jobs");

            //migrationBuilder.AddColumn<float>(
            //    name: "MaxSalary",
            //    table: "Jobs",
            //    type: "real",
            //    nullable: false,
            //    defaultValue: 0f);

            //migrationBuilder.AddColumn<float>(
            //    name: "MinSalary",
            //    table: "Jobs",
            //    type: "real",
            //    nullable: false,
            //    defaultValue: 0f);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Applicants_ApplicantId",
                table: "Applications",
                column: "ApplicantId",
                principalTable: "Applicants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Applicants_ApplicantId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "MinSalary",
                table: "Jobs");

            //migrationBuilder.AddColumn<double>(
            //    name: "Salary",
            //    table: "Jobs",
            //    type: "float",
            //    nullable: false,
            //    defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Applicants_ApplicantId",
                table: "Applications",
                column: "ApplicantId",
                principalTable: "Applicants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Jobs_JobId",
                table: "Applications",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");
        }
    }
}
