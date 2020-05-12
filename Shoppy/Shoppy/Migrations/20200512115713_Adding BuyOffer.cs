using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shoppy.Migrations
{
    public partial class AddingBuyOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanReciveBuyOffers",
                table: "SellOffers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BuyOffer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OfferedMoney = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SellOfferId = table.Column<int>(nullable: false),
                    BuyerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyOffer_SellOffers_SellOfferId",
                        column: x => x.SellOfferId,
                        principalTable: "SellOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyOffer_SellOfferId",
                table: "BuyOffer",
                column: "SellOfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyOffer");

            migrationBuilder.DropColumn(
                name: "CanReciveBuyOffers",
                table: "SellOffers");
        }
    }
}
