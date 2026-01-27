using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncingModelWithDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Commodities_CommodityTypes_CommodityTypeId",
                table: "Commodities",
                column: "CommodityTypeId",
                principalTable: "CommodityTypes",
                principalColumn: "CommodityTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
