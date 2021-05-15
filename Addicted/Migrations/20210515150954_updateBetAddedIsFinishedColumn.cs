using Microsoft.EntityFrameworkCore.Migrations;

namespace Addicted.Migrations
{
    public partial class updateBetAddedIsFinishedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Bets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Bets");
        }
    }
}
