using Microsoft.EntityFrameworkCore.Migrations;

namespace Addicted.Migrations
{
    public partial class newCoinTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoinsId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaciusCoin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coin", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CoinsId",
                table: "AspNetUsers",
                column: "CoinsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Coin_CoinsId",
                table: "AspNetUsers",
                column: "CoinsId",
                principalTable: "Coin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Coin_CoinsId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Coin");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CoinsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CoinsId",
                table: "AspNetUsers");
        }
    }
}
