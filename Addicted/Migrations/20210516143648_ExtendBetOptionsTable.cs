using Microsoft.EntityFrameworkCore.Migrations;

namespace Addicted.Migrations
{
    public partial class ExtendBetOptionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWinner",
                table: "BetOption",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "BetOption");
        }
    }
}
