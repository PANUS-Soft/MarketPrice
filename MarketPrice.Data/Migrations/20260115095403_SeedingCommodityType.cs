using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingCommodityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        -- 1. Get the GUIDs for the Units of Measure
        DECLARE @KgId UNIQUEIDENTIFIER = (SELECT TOP 1 UnitOfMeasureId FROM UnitOfMeasures WHERE UnitOfMeasureCodeEnglish = 'kg');
        DECLARE @LitreId UNIQUEIDENTIFIER = (SELECT TOP 1 UnitOfMeasureId FROM UnitOfMeasures WHERE UnitOfMeasureCodeEnglish = 'L');

        -- 2. Define Lookup Type IDs based on your schema
        DECLARE @GroupType INT = 2000; 
        DECLARE @NameType INT = 3000;  

        -- 3. Insert Commodity Types
        -- CORN (Group: Grain, Name: Corn)
        INSERT INTO [CommodityTypes] (CommodityTypeId, CommodityGroupId, NameId, Code, DefaultUnitOfMeasureId)
        VALUES (
            NEWID(), 
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Grain' AND LookupDataTypeId = @GroupType),
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Corn' AND LookupDataTypeId = @NameType),
            'CORN', @KgId
        );

        -- BEANS (Group: Legumes, Name: Beans)
        INSERT INTO [CommodityTypes] (CommodityTypeId, CommodityGroupId, NameId, Code, DefaultUnitOfMeasureId)
        VALUES (
            NEWID(), 
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Legumes' AND LookupDataTypeId = @GroupType),
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Beans' AND LookupDataTypeId = @NameType),
            'BEAN', @KgId
        );

        -- PALM OIL (Group: Oil, Name: Palm Oil)
        INSERT INTO [CommodityTypes] (CommodityTypeId, CommodityGroupId, NameId, Code, DefaultUnitOfMeasureId)
        VALUES (
            NEWID(), 
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Oil' AND LookupDataTypeId = @GroupType),
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Palm Oil' AND LookupDataTypeId = @NameType),
            'POIL', @LitreId
        );

        -- ONION (Group: Vegetable, Name: Onion)
        INSERT INTO [CommodityTypes] (CommodityTypeId, CommodityGroupId, NameId, Code, DefaultUnitOfMeasureId)
        VALUES (
            NEWID(), 
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Vegetable' AND LookupDataTypeId = @GroupType),
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Onion' AND LookupDataTypeId = @NameType),
            'ONIO', @KgId
        );
        
        -- GINGER (Group: Root, Name: Ginger)
        INSERT INTO [CommodityTypes] (CommodityTypeId, CommodityGroupId, NameId, Code, DefaultUnitOfMeasureId)
        VALUES (
            NEWID(), 
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Root' AND LookupDataTypeId = @GroupType),
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Ginger' AND LookupDataTypeId = @NameType),
            'GING', @KgId
        );

        -- EGUSI (Group: OilSeed, Name: Egusi)
        INSERT INTO [CommodityTypes] (CommodityTypeId, CommodityGroupId, NameId, Code, DefaultUnitOfMeasureId)
        VALUES (
            NEWID(), 
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'OilSeed' AND LookupDataTypeId = @GroupType),
            (SELECT LookupDataId FROM LookupData WHERE LookupDataTextEnglish = 'Egusi' AND LookupDataTypeId = @NameType),
            'EGUS', @KgId
        );
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [CommodityTypes] WHERE Code IN ('CORN', 'BEAN', 'POIL', 'ONIO', 'GING', 'EGUS')");
        }
    }
}
