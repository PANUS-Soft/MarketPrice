using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBestPriceInCOmmodityAndCommodityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "CommodityTypes",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "LastBestBid",
                table: "CommodityTypes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastBestOffer",
                table: "CommodityTypes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateUpdated",
                table: "Commodities",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<decimal>(
                name: "LastBestBid",
                table: "Commodities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastBestOffer",
                table: "Commodities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "CommodityTypes");

            migrationBuilder.DropColumn(
                name: "LastBestBid",
                table: "CommodityTypes");

            migrationBuilder.DropColumn(
                name: "LastBestOffer",
                table: "CommodityTypes");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Commodities");

            migrationBuilder.DropColumn(
                name: "LastBestBid",
                table: "Commodities");

            migrationBuilder.DropColumn(
                name: "LastBestOffer",
                table: "Commodities");
        }
    }
}
