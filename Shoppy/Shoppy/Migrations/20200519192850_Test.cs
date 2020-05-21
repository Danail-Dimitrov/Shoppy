using Microsoft.EntityFrameworkCore.Migrations;

namespace Shoppy.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyOffer_SellOffers_SellOfferId",
                table: "BuyOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyOffer_AspNetUsers_UserId",
                table: "BuyOffer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuyOffer",
                table: "BuyOffer");

            migrationBuilder.RenameTable(
                name: "BuyOffer",
                newName: "BuyOffers");

            migrationBuilder.RenameIndex(
                name: "IX_BuyOffer_UserId",
                table: "BuyOffers",
                newName: "IX_BuyOffers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BuyOffer_SellOfferId",
                table: "BuyOffers",
                newName: "IX_BuyOffers_SellOfferId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuyOffers",
                table: "BuyOffers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyOffers_SellOffers_SellOfferId",
                table: "BuyOffers",
                column: "SellOfferId",
                principalTable: "SellOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BuyOffers_AspNetUsers_UserId",
                table: "BuyOffers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyOffers_SellOffers_SellOfferId",
                table: "BuyOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyOffers_AspNetUsers_UserId",
                table: "BuyOffers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BuyOffers",
                table: "BuyOffers");

            migrationBuilder.RenameTable(
                name: "BuyOffers",
                newName: "BuyOffer");

            migrationBuilder.RenameIndex(
                name: "IX_BuyOffers_UserId",
                table: "BuyOffer",
                newName: "IX_BuyOffer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BuyOffers_SellOfferId",
                table: "BuyOffer",
                newName: "IX_BuyOffer_SellOfferId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BuyOffer",
                table: "BuyOffer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyOffer_SellOffers_SellOfferId",
                table: "BuyOffer",
                column: "SellOfferId",
                principalTable: "SellOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BuyOffer_AspNetUsers_UserId",
                table: "BuyOffer",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
