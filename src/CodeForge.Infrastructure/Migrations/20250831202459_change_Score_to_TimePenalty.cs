using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeforge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class change_Score_to_TimePenalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Standings");

            migrationBuilder.AddColumn<int>(
                name: "TimePenalty",
                table: "Standings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimePenalty",
                table: "Standings");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Standings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
