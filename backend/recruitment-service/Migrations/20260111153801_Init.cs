using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecruitmentService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CandidateId = table.Column<int>(type: "integer", nullable: false),
                    Answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Questions = table.Column<List<string>>(type: "text[]", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    StudyType = table.Column<int>(type: "integer", nullable: false),
                    StudyLanguage = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    FacebookProfile = table.Column<string>(type: "text", nullable: true),
                    InstagramProfile = table.Column<string>(type: "text", nullable: true),
                    Diet = table.Column<int>(type: "integer", nullable: false),
                    Allergies = table.Column<string>(type: "text", nullable: true),
                    ShirtSize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InterviewTemplateId = table.Column<int>(type: "integer", nullable: false),
                    RecruitmentFormTemplateId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentFormTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Questions = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentFormTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerDisponibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VolunteerId = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerDisponibilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PersonalEmail = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    PersonalInfoId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    VolunteerStatus = table.Column<int>(type: "integer", nullable: false),
                    Department = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Volunteers_PersonalInfo_PersonalInfoId",
                        column: x => x.PersonalInfoId,
                        principalTable: "PersonalInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockedPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    RecruitmentCampaignId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockedPeriods_RecruitmentCampaigns_RecruitmentCampaignId",
                        column: x => x.RecruitmentCampaignId,
                        principalTable: "RecruitmentCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PersonalEmail = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    PersonalInfoId = table.Column<int>(type: "integer", nullable: false),
                    RecruitingStatus = table.Column<int>(type: "integer", nullable: false),
                    RecruitmentCampaignId = table.Column<int>(type: "integer", nullable: false),
                    AnswersToForm = table.Column<List<string>>(type: "text[]", nullable: false),
                    SchedulerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidates_PersonalInfo_PersonalInfoId",
                        column: x => x.PersonalInfoId,
                        principalTable: "PersonalInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidates_RecruitmentCampaigns_RecruitmentCampaignId",
                        column: x => x.RecruitmentCampaignId,
                        principalTable: "RecruitmentCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    RecruitmentCampaignId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_RecruitmentCampaigns_RecruitmentCampaignId",
                        column: x => x.RecruitmentCampaignId,
                        principalTable: "RecruitmentCampaigns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InterviewVolunteer",
                columns: table => new
                {
                    InterviewId = table.Column<int>(type: "integer", nullable: false),
                    InterviewersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewVolunteer", x => new { x.InterviewId, x.InterviewersId });
                    table.ForeignKey(
                        name: "FK_InterviewVolunteer_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewVolunteer_Volunteers_InterviewersId",
                        column: x => x.InterviewersId,
                        principalTable: "Volunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentCampaignVolunteer",
                columns: table => new
                {
                    RecruitmentCampaignId = table.Column<int>(type: "integer", nullable: false),
                    VolunteersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentCampaignVolunteer", x => new { x.RecruitmentCampaignId, x.VolunteersId });
                    table.ForeignKey(
                        name: "FK_RecruitmentCampaignVolunteer_RecruitmentCampaigns_Recruitme~",
                        column: x => x.RecruitmentCampaignId,
                        principalTable: "RecruitmentCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecruitmentCampaignVolunteer_Volunteers_VolunteersId",
                        column: x => x.VolunteersId,
                        principalTable: "Volunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockedPeriods_RecruitmentCampaignId",
                table: "BlockedPeriods",
                column: "RecruitmentCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_PersonalInfoId",
                table: "Candidates",
                column: "PersonalInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_RecruitmentCampaignId",
                table: "Candidates",
                column: "RecruitmentCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_LocationId_DateTime",
                table: "Interviews",
                columns: new[] { "LocationId", "DateTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewVolunteer_InterviewersId",
                table: "InterviewVolunteer",
                column: "InterviewersId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RecruitmentCampaignId",
                table: "Locations",
                column: "RecruitmentCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentCampaigns_Name",
                table: "RecruitmentCampaigns",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentCampaignVolunteer_VolunteersId",
                table: "RecruitmentCampaignVolunteer",
                column: "VolunteersId");

            migrationBuilder.CreateIndex(
                name: "IX_Volunteers_PersonalInfoId",
                table: "Volunteers",
                column: "PersonalInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedPeriods");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "InterviewTemplates");

            migrationBuilder.DropTable(
                name: "InterviewVolunteer");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "RecruitmentCampaignVolunteer");

            migrationBuilder.DropTable(
                name: "RecruitmentFormTemplates");

            migrationBuilder.DropTable(
                name: "VolunteerDisponibilities");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "RecruitmentCampaigns");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "PersonalInfo");
        }
    }
}
