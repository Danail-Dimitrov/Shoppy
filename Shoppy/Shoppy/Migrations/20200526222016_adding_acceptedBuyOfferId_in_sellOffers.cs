using Microsoft.EntityFrameworkCore.Migrations;

namespace Shoppy.Migrations
{
    public partial class adding_acceptedBuyOfferId_in_sellOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcceptedBuyOfferId",
                table: "SellOffers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedBuyOfferId",
                table: "SellOffers");
        }
    }
}
