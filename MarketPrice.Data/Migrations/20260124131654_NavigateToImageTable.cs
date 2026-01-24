using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class NavigateToImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commodities_CommodityImage_CommodityImageId",
                table: "Commodities");

            migrationBuilder.AddForeignKey(
                name: "FK_Commodities_CommodityImage_CommodityImageId",
                table: "Commodities",
                column: "CommodityImageId",
                principalTable: "CommodityImage",
                principalColumn: "CommodityImageId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commodities_CommodityImage_CommodityImageId",
                table: "Commodities");

            migrationBuilder.AddForeignKey(
                name: "FK_Commodities_CommodityImage_CommodityImageId",
                table: "Commodities",
                column: "CommodityImageId",
                principalTable: "CommodityImage",
                principalColumn: "CommodityImageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
