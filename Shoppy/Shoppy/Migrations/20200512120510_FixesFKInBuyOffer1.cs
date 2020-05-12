using Microsoft.EntityFrameworkCore.Migrations;

namespace Shoppy.Migrations
{
    public partial class FixesFKInBuyOffer1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "BuyOffer");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BuyOffer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BuyOffer_UserId",
                table: "BuyOffer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyOffer_AspNetUsers_UserId",
                table: "BuyOffer",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyOffer_AspNetUsers_UserId",
                table: "BuyOffer");

            migrationBuilder.DropIndex(
                name: "IX_BuyOffer_UserId",
                table: "BuyOffer");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BuyOffer");

            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "BuyOffer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
