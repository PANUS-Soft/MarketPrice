using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCommodityTypeImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seeding CommodityTypeImage
            migrationBuilder.Sql(@"
                -- BEANS
                INSERT INTO CommodityTypeImage
                (CommodityTypeImageId, CommodityTypeId, ImageData, ContentType, FileName)
                SELECT
                    NEWID(),
                    ct.CommodityTypeId, -- Assuming this is the correct reference
                    BulkColumn,
                    'image/png',
                    'beans.png'
                FROM CommodityTypes ct
                CROSS APPLY OPENROWSET(
                    BULK 'C:\MarketPrice\MarketPrice.Data\Images\CommodityTypes\beans.png',
                    SINGLE_BLOB
                ) AS Image
                WHERE ct.Code = 'BNS';

                -- ONIONS
                INSERT INTO CommodityTypeImage
                (CommodityTypeImageId, CommodityTypeId, ImageData, ContentType, FileName)
                SELECT
                    NEWID(),
                    ct.CommodityTypeId,
                    BulkColumn,
                    'image/png',
                    'onion.png'
                FROM CommodityTypes ct
                CROSS APPLY OPENROWSET(
                    BULK 'C:\MarketPrice\MarketPrice.Data\Images\CommodityTypes\onion.png',
                    SINGLE_BLOB
                ) AS Image
                WHERE ct.Code = 'ONI';

                -- GINGER
                INSERT INTO CommodityTypeImage
                (CommodityTypeImageId, CommodityTypeId, ImageData, ContentType, FileName)
                SELECT
                    NEWID(),
                    ct.CommodityTypeId,
                    BulkColumn,
                    'image/png',
                    'ginger.png'
                FROM CommodityTypes ct
                CROSS APPLY OPENROWSET(
                    BULK 'C:\MarketPrice\MarketPrice.Data\Images\CommodityTypes\ginger.png',
                    SINGLE_BLOB
                ) AS Image
                WHERE ct.Code = 'GIN';

                -- CORN
                INSERT INTO CommodityTypeImage
                (CommodityTypeImageId, CommodityTypeId, ImageData, ContentType, FileName)
                SELECT
                    NEWID(),
                    ct.CommodityTypeId,
                    BulkColumn,
                    'image/png',
                    'corn.png'
                FROM CommodityTypes ct
                CROSS APPLY OPENROWSET(
                    BULK 'C:\MarketPrice\MarketPrice.Data\Images\CommodityTypes\corn.png',
                    SINGLE_BLOB
                ) AS Image
                WHERE ct.Code = 'CRN';

                -- PALM OIL
                INSERT INTO CommodityTypeImage
                (CommodityTypeImageId, CommodityTypeId, ImageData, ContentType, FileName)
                SELECT
                    NEWID(),
                    ct.CommodityTypeId,
                    BulkColumn,
                    'image/png',
                    'palm_oil.png'
                FROM CommodityTypes ct
                CROSS APPLY OPENROWSET(
                    BULK 'C:\MarketPrice\MarketPrice.Data\Images\CommodityTypes\palm_oil.png',
                    SINGLE_BLOB
                ) AS Image
                WHERE ct.Code = 'OIL';

                -- EGUSI
                INSERT INTO CommodityTypeImage
                (CommodityTypeImageId, CommodityTypeId, ImageData, ContentType, FileName)
                SELECT
                    NEWID(),
                    ct.CommodityTypeId,
                    BulkColumn,
                    'image/png',
                    'egusi.png'
                FROM CommodityTypes ct
                CROSS APPLY OPENROWSET(
                    BULK 'C:\MarketPrice\MarketPrice.Data\Images\CommodityTypes\egusi.png',
                    SINGLE_BLOB
                ) AS Image
                WHERE ct.Code = 'EGU';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM CommodityTypeImage
                WHERE FileName IN (
                    'beans.png',
                    'onion.png',
                    'ginger.png',
                    'corn.png',
                    'palm_oil.png',
                    'egusi.png'
                );
            ");
        }
    }
}
