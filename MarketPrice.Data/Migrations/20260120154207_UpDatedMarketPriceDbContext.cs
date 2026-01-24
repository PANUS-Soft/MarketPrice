using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpDatedMarketPriceDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Commodities_CommodityId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Users_UserId",
                table: "Positions");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Commodities_CommodityId",
                table: "Positions",
                column: "CommodityId",
                principalTable: "Commodities",
                principalColumn: "CommodityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Users_UserId",
                table: "Positions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Commodities_CommodityId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Users_UserId",
                table: "Positions");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Commodities_CommodityId",
                table: "Positions",
                column: "CommodityId",
                principalTable: "Commodities",
                principalColumn: "CommodityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Users_UserId",
                table: "Positions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
