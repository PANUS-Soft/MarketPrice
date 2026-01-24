using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommodityImageAndCommodityTypeImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create CommodityImage table
            migrationBuilder.CreateTable(
                name: "CommodityImage",
                columns: table => new
                {
                    CommodityImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CommodityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityImage", x => x.CommodityImageId);
                    table.ForeignKey(
                        name: "FK_CommodityImage_Commodities_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodities",
                        principalColumn: "CommodityId",
                        onDelete: ReferentialAction.Cascade); 
                });

            // Create CommodityTypeImage table
            migrationBuilder.CreateTable(
                name: "CommodityTypeImage",
                columns: table => new
                {
                    CommodityTypeImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CommodityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityTypeImage", x => x.CommodityTypeImageId);
                    table.ForeignKey(
                        name: "FK_CommodityTypeImage_CommodityTypes_CommodityTypeId",
                        column: x => x.CommodityTypeId,
                        principalTable: "CommodityTypes",
                        principalColumn: "CommodityTypeId",
                        onDelete: ReferentialAction.Cascade); 
                });

            // Create indexes for foreign keys
            migrationBuilder.CreateIndex(
                name: "IX_CommodityImage_CommodityId",
                table: "CommodityImage",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTypeImage_CommodityTypeId",
                table: "CommodityTypeImage",
                column: "CommodityTypeId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "CommodityImage");

            migrationBuilder.DropTable(
                name: "CommodityTypeImage");
        }
    }
}
