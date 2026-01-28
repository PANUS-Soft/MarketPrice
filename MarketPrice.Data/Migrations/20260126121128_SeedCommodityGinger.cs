using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCommodityGinger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Insert image for 'Dry Ginger'
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'ginger.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\ginger.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Dry Ginger';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM CommodityImage 
                WHERE FileName = 'ginger.png' 
                AND CommodityId IN (SELECT CommodityId FROM Commodities WHERE CommodityName = 'Dry Ginger');
            ");
        }
    }
}
