using Microsoft.EntityFrameworkCore.Migrations;

namespace Shoppy.Migrations
{
    public partial class adding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedBuyOffer",
                table: "SellOffers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAcceptedBuyOffer",
                table: "SellOffers");
        }
    }
}
