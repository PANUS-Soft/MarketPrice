using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyMarketDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Commodities_CommodityTypes_CommodityTypeId",
            //    table: "Commodities");

            //migrationBuilder.DropIndex(
            //    name: "IX_Commodities_CommodityTypeId",
            //    table: "Commodities");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LastBestOffer",
            //    table: "Commodities",
            //    type: "decimal(18,4)",
            //    precision: 18,
            //    scale: 4,
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LastBestBid",
            //    table: "Commodities",
            //    type: "decimal(18,4)",
            //    precision: 18,
            //    scale: 4,
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");

            //migrationBuilder.CreateTable(
            //    name: "CommodityImage",
            //    columns: table => new
            //    {
            //        CommodityImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        CommodityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            //        ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CommodityImage", x => x.CommodityImageId);
            //        table.ForeignKey(
            //            name: "FK_CommodityImage_Commodities_CommodityId",
            //            column: x => x.CommodityId,
            //            principalTable: "Commodities",
            //            principalColumn: "CommodityId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CommodityTypeImage",
            //    columns: table => new
            //    {
            //        CommodityTypeImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        CommodityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
            //        ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CommodityTypeImage", x => x.CommodityTypeImageId);
            //        table.ForeignKey(
            //            name: "FK_CommodityTypeImage_CommodityTypes_CommodityTypeId",
            //            column: x => x.CommodityTypeId,
            //            principalTable: "CommodityTypes",
            //            principalColumn: "CommodityTypeId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_CommodityImage_CommodityId",
            //    table: "CommodityImage",
            //    column: "CommodityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CommodityTypeImage_CommodityTypeId",
            //    table: "CommodityTypeImage",
            //    column: "CommodityTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "CommodityImage");

            //migrationBuilder.DropTable(
            //    name: "CommodityTypeImage");

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LastBestOffer",
            //    table: "Commodities",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,4)",
            //    oldPrecision: 18,
            //    oldScale: 4);

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "LastBestBid",
            //    table: "Commodities",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,4)",
            //    oldPrecision: 18,
            //    oldScale: 4);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Commodities_CommodityTypeId",
            //    table: "Commodities",
            //    column: "CommodityTypeId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Commodities_CommodityTypes_CommodityTypeId",
            //    table: "Commodities",
            //    column: "CommodityTypeId",
            //    principalTable: "CommodityTypes",
            //    principalColumn: "CommodityTypeId",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
