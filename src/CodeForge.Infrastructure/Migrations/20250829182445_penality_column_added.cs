using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeforge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class penality_column_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Penalty",
                table: "Submissions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Penalty",
                table: "Submissions");
        }
    }
}
