using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeforge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Unused_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Standings");

            migrationBuilder.DropColumn(
                name: "ProblemLabel",
                table: "ProblemResults");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "ProblemResults");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Standings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProblemLabel",
                table: "ProblemResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "ProblemResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
