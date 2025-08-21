using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rezk_Proj.Migrations
{
    /// <inheritdoc />
    public partial class seedroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName" ,"ConcurrencyStamp"},
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), "Admin", "ADMIN",Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Employer", "EMPLOYER",Guid.NewGuid().ToString() },
                    { Guid.NewGuid().ToString(), "Applicant", "APPLICANT",Guid.NewGuid().ToString() }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles]");
        }
    }
}
