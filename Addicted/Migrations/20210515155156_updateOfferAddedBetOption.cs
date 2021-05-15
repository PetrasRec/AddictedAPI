using Microsoft.EntityFrameworkCore.Migrations;

namespace Addicted.Migrations
{
    public partial class updateOfferAddedBetOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BetOptionId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offer_BetOptionId",
                table: "Offer",
                column: "BetOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_BetOption_BetOptionId",
                table: "Offer",
                column: "BetOptionId",
                principalTable: "BetOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_BetOption_BetOptionId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_BetOptionId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "BetOptionId",
                table: "Offer");
        }
    }
}
