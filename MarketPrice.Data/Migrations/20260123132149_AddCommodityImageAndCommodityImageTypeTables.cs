using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommodityImageAndCommodityImageTypeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommodityTypeImageId",
                table: "CommodityTypes",
                type: "uniqueidentifier",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "CommodityImageId",
                table: "Commodities",
                type: "uniqueidentifier",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.CreateTable(
                name: "CommodityImage",
                columns: table => new
                {
                    CommodityImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityImage", x => x.CommodityImageId);
                });

            migrationBuilder.CreateTable(
                name: "CommodityTypeImage",
                columns: table => new
                {
                    CommodityTypeImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityTypeImage", x => x.CommodityTypeImageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTypes_CommodityTypeImageId",
                table: "CommodityTypes",
                column: "CommodityTypeImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Commodities_CommodityImageId",
                table: "Commodities",
                column: "CommodityImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commodities_CommodityImage_CommodityImageId",
                table: "Commodities",
                column: "CommodityImageId",
                principalTable: "CommodityImage",
                principalColumn: "CommodityImageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommodityTypes_CommodityTypeImage_CommodityTypeImageId",
                table: "CommodityTypes",
                column: "CommodityTypeImageId",
                principalTable: "CommodityTypeImage",
                principalColumn: "CommodityTypeImageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commodities_CommodityImage_CommodityImageId",
                table: "Commodities");

            migrationBuilder.DropForeignKey(
                name: "FK_CommodityTypes_CommodityTypeImage_CommodityTypeImageId",
                table: "CommodityTypes");

            migrationBuilder.DropTable(
                name: "CommodityImage");

            migrationBuilder.DropTable(
                name: "CommodityTypeImage");

            migrationBuilder.DropIndex(
                name: "IX_CommodityTypes_CommodityTypeImageId",
                table: "CommodityTypes");

            migrationBuilder.DropIndex(
                name: "IX_Commodities_CommodityImageId",
                table: "Commodities");

            migrationBuilder.DropColumn(
                name: "CommodityTypeImageId",
                table: "CommodityTypes");

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
                name: "CommodityImageId",
                table: "Commodities");

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
