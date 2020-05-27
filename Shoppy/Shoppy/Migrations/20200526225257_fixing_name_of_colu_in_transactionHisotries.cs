using Microsoft.EntityFrameworkCore.Migrations;

namespace Shoppy.Migrations
{
    public partial class fixing_name_of_colu_in_transactionHisotries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "TransactionHistories");

            migrationBuilder.AddColumn<string>(
                name: "ProductTitle",
                table: "TransactionHistories",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductTitle",
                table: "TransactionHistories");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "TransactionHistories",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
