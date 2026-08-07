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
        -- Grains / Corn (Updated to 50kg)
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @CornType, @KgId, 'Dry Corn', 50, 365, 'High starch content'),
               (NEWID(), @CornType, @KgId, 'Fresh Corn', 50, 7, 'Perishable; keep cool');

        -- Legumes / Beans (Updated to 50kg)
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @BeanType, @KgId, 'Red Beans', 50, 730, 'Very Good when fried'),
               (NEWID(), @BeanType, @KgId, 'White Beans', 50, 730, 'Commonly eaten with tubers or rice'),
               (NEWID(), @BeanType, @KgId, 'Black Beans', 50, 730, 'Good in pounded meals like pounded Irish potatoes');

        -- Vegetables / Onions (Updated to 50kg)
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @OnionType, @KgId, 'White Onions', 50, 30, 'Mild flavor'),
               (NEWID(), @OnionType, @KgId, 'Red Onions', 50, 60, 'Sharp flavor; stores longer');

        -- Roots / Ginger (Updated to 25kg)
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @GingerType, @KgId, 'Dry Ginger', 25, 365, 'Dehydrated roots');

        -- Oilseeds / Egusi (Updated to 25kg)
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @EgusiType, @KgId, 'Uncracked Egusi', 25, 365, 'Protected by shell'),
               (NEWID(), @EgusiType, @KgId, 'Cracked Egusi', 25, 90, 'Shelled; needs dry storage');

        -- Oils / Palm Oil (Updated to 20L)
        INSERT INTO [Commodities] (CommodityId, CommodityTypeId, UnitOfMeasureId, CommodityName, LotSize, ShelfLifeInDays, Notes)
        VALUES (NEWID(), @OilType, @LitreId, 'Palm Oil', 20, 545, 'Pure extract');
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clean up all seeded commodities
            migrationBuilder.Sql("DELETE FROM [Commodities]");
        }
    }
}