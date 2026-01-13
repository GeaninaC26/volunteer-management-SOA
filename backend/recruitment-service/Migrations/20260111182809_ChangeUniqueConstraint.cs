using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Interviews_LocationId_DateTime",
                table: "Interviews");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CandidateId_LocationId_DateTime",
                table: "Interviews",
                columns: new[] { "CandidateId", "LocationId", "DateTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Interviews_CandidateId_LocationId_DateTime",
                table: "Interviews");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_LocationId_DateTime",
                table: "Interviews",
                columns: new[] { "LocationId", "DateTime" },
                unique: true);
        }
    }
}
