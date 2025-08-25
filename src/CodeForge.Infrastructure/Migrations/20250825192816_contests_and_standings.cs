using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeforge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class contests_and_standings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "Submissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContestId",
                table: "Submissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContestId",
                table: "Problems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContestParticipants",
                columns: table => new
                {
                    ContestsId = table.Column<int>(type: "int", nullable: false),
                    ParticipantsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestParticipants", x => new { x.ContestsId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_ContestParticipants_AspNetUsers_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestParticipants_Contests_ContestsId",
                        column: x => x.ContestsId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Standings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Standings_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StandingId = table.Column<int>(type: "int", nullable: false),
                    ProblemLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemResults_Standings_StandingId",
                        column: x => x.StandingId,
                        principalTable: "Standings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ContestId",
                table: "Submissions",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ContestId",
                table: "Problems",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_ContestParticipants_ParticipantsId",
                table: "ContestParticipants",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemResults_StandingId",
                table: "ProblemResults",
                column: "StandingId");

            migrationBuilder.CreateIndex(
                name: "IX_Standings_ContestId",
                table: "Standings",
                column: "ContestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Contests_ContestId",
                table: "Problems",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Contests_ContestId",
                table: "Submissions",
                column: "ContestId",
                principalTable: "Contests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Contests_ContestId",
                table: "Problems");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Contests_ContestId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "ContestParticipants");

            migrationBuilder.DropTable(
                name: "ProblemResults");

            migrationBuilder.DropTable(
                name: "Standings");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ContestId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ContestId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "ContestId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ContestId",
                table: "Problems");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "Submissions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
