using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCommodityImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- BLACK_BEANS
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'black_beans.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\black_beans.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Black Beans';

                -- CRACKED_EGUSI
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'cracked_egusi.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\cracked_egusi.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Cracked Egusi';

                -- DRY_CORN
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'dry_corn.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\dry_corn.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Dry Corn';

                -- FRESH_CORN
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'fresh_corn.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\fresh_corn.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Fresh Corn';

                -- GINGER
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'ginger.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\ginger.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Ginger';

                -- PALM_OIL
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'palm_oil.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\palm_oil.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Palm Oil';

                -- RED_BEANS
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'red_beans.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\red_beans.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Red Beans';

                -- RED_ONION
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'red_onion.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\red_onion.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Red Onions';

                -- UNCRACKED_EGUSI
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'uncracked_egusi.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\uncracked_egusi.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'Uncracked Egusi';

                -- WHITE_BEANS
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'white_beans.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\white_beans.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'White Beans';

                -- WHITE_ONION
                INSERT INTO CommodityImage (CommodityImageId, CommodityId, ImageData, ContentType, FileName)
                SELECT NEWID(), c.CommodityId, Image.BulkColumn, 'image/png', 'white_onion.png'
                FROM Commodities c
                CROSS APPLY OPENROWSET(BULK 'C:\MarketPrice\MarketPrice.Data\Images\Commodities\white_onion.png', SINGLE_BLOB) AS Image
                WHERE c.CommodityName = 'White Onions';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM CommodityImage
                WHERE FileName IN (
                    'black_beans.png',
                    'cracked_egusi.png',
                    'dry_corn.png',
                    'fresh_corn.png',
                    'ginger.png',
                    'palm_oil.png',
                    'red_beans.png',
                    'red_onion.png',
                    'uncracked_egusi.png',
                    'white_beans.png',
                    'white_onion.png'
                );
            ");
        }
    }
}
