using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentService.Migrations
{
    /// <inheritdoc />
    public partial class addrule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_RecruitmentCampaigns_RecruitmentCampaignId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_RecruitmentCampaignId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "RecruitmentCampaignId",
                table: "Locations");

            migrationBuilder.CreateTable(
                name: "LocationRecruitmentCampaign",
                columns: table => new
                {
                    LocationsId = table.Column<int>(type: "integer", nullable: false),
                    RecruitmentCampaignId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationRecruitmentCampaign", x => new { x.LocationsId, x.RecruitmentCampaignId });
                    table.ForeignKey(
                        name: "FK_LocationRecruitmentCampaign_Locations_LocationsId",
                        column: x => x.LocationsId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationRecruitmentCampaign_RecruitmentCampaigns_Recruitmen~",
                        column: x => x.RecruitmentCampaignId,
                        principalTable: "RecruitmentCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationRecruitmentCampaign_RecruitmentCampaignId",
                table: "LocationRecruitmentCampaign",
                column: "RecruitmentCampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationRecruitmentCampaign");

            migrationBuilder.AddColumn<int>(
                name: "RecruitmentCampaignId",
                table: "Locations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RecruitmentCampaignId",
                table: "Locations",
                column: "RecruitmentCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_RecruitmentCampaigns_RecruitmentCampaignId",
                table: "Locations",
                column: "RecruitmentCampaignId",
                principalTable: "RecruitmentCampaigns",
                principalColumn: "Id");
        }
    }
}
