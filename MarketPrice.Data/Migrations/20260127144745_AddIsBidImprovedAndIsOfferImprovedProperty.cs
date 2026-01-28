using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBidImprovedAndIsOfferImprovedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBidImproved",
                table: "CommodityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfferImproved",
                table: "CommodityTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBidImproved",
                table: "Commodities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfferImproved",
                table: "Commodities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBidImproved",
                table: "CommodityTypes");

            migrationBuilder.DropColumn(
                name: "IsOfferImproved",
                table: "CommodityTypes");

            migrationBuilder.DropColumn(
                name: "IsBidImproved",
                table: "Commodities");

            migrationBuilder.DropColumn(
                name: "IsOfferImproved",
                table: "Commodities");
        }
    }
}
