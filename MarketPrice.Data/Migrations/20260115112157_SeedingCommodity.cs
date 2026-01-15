using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingCommodity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        -- 1. Declare variables to hold the Type IDs
        DECLARE @CornType UNIQUEIDENTIFIER = (SELECT CommodityTypeId FROM CommodityTypes WHERE Code = 'CORN');
        DECLARE @BeanType UNIQUEIDENTIFIER = (SELECT CommodityTypeId FROM CommodityTypes WHERE Code = 'BEAN');
        DECLARE @OnionType UNIQUEIDENTIFIER = (SELECT CommodityTypeId FROM CommodityTypes WHERE Code = 'ONIO');
        DECLARE @GingerType UNIQUEIDENTIFIER = (SELECT CommodityTypeId FROM CommodityTypes WHERE Code = 'GING');
        DECLARE @EgusiType UNIQUEIDENTIFIER = (SELECT CommodityTypeId FROM CommodityTypes WHERE Code = 'EGUS');
        DECLARE @OilType UNIQUEIDENTIFIER = (SELECT CommodityTypeId FROM CommodityTypes WHERE Code = 'POIL');

        -- 2. Declare variables for Unit of Measure IDs
        DECLARE @KgId UNIQUEIDENTIFIER = (SELECT TOP 1 UnitOfMeasureId FROM UnitOfMeasures WHERE UnitOfMeasureCodeEnglish = 'kg');
        DECLARE @LitreId UNIQUEIDENTIFIER = (SELECT TOP 1 UnitOfMeasureId FROM UnitOfMeasures WHERE UnitOfMeasureCodeEnglish = 'L');

        -- 3 Insert specific Commodities
        -- Grains / Corn
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @CornType, @KgId, 'Dry Corn', 100, 365, 'High starch content'),
               (NEWID(), @CornType, @KgId, 'Fresh Corn', 50, 7, 'Perishable; keep cool');

        -- Legumes / Beans
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @BeanType, @KgId, 'Red Beans', 100, 730, 'Very Good when fried'),
               (NEWID(), @BeanType, @KgId, 'White Beans', 100, 730, 'Commonly eaten with tubers or rice'),
               (NEWID(), @BeanType, @KgId, 'Black Beans', 100, 730, 'Good in pounded meals like pounded Irish potatoes');

        -- Vegetables / Onions
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @OnionType, @KgId, 'White Onions', 40, 30, 'Mild flavor'),
               (NEWID(), @OnionType, @KgId, 'Red Onions', 40, 60, 'Sharp flavor; stores longer');

        -- Roots / Ginger
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @GingerType, @KgId, 'Dry Ginger', 30, 365, 'Dehydrated roots');

        -- Oilseeds / Egusi
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @EgusiType, @KgId, 'Uncracked Egusi', 50, 365, 'Protected by shell'),
               (NEWID(), @EgusiType, @KgId, 'Cracked Egusi', 20, 90, 'Shelled; needs dry storage');

        -- Oils / Palm Oil
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @OilType, @LitreId, 'Palm Oil', 25, 545, 'Pure extract');
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clean up all seeded commodities
            migrationBuilder.Sql("DELETE FROM [Commodities]");
        }
    }
}
