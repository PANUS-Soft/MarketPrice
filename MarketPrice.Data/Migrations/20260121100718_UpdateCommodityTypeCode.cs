using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommodityTypeCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @NameType INT = 3000;

                -- CORN → CRN
                UPDATE ct
                SET ct.Code = 'CRN'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Corn'
                  AND ld.LookupDataTypeId = @NameType;

                -- BEANS → BNS
                UPDATE ct
                SET ct.Code = 'BNS'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Beans'
                  AND ld.LookupDataTypeId = @NameType;

                -- EGUSI → EGU
                UPDATE ct
                SET ct.Code = 'EGU'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Egusi'
                  AND ld.LookupDataTypeId = @NameType;

                -- GINGER → GIN
                UPDATE ct
                SET ct.Code = 'GIN'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Ginger'
                  AND ld.LookupDataTypeId = @NameType;

                -- ONION → ONI
                UPDATE ct
                SET ct.Code = 'ONI'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Onion'
                  AND ld.LookupDataTypeId = @NameType;

                -- PALM OIL → OIL
                UPDATE ct
                SET ct.Code = 'OIL'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Palm Oil'
                  AND ld.LookupDataTypeId = @NameType;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @NameType INT = 3000;

                UPDATE ct SET ct.Code = 'CORN'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Corn'
                  AND ld.LookupDataTypeId = @NameType;

                UPDATE ct SET ct.Code = 'BEAN'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Beans'
                  AND ld.LookupDataTypeId = @NameType;

                UPDATE ct SET ct.Code = 'EGUS'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Egusi'
                  AND ld.LookupDataTypeId = @NameType;

                UPDATE ct SET ct.Code = 'GING'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Ginger'
                  AND ld.LookupDataTypeId = @NameType;

                UPDATE ct SET ct.Code = 'ONIO'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Onion'
                  AND ld.LookupDataTypeId = @NameType;

                UPDATE ct SET ct.Code = 'POIL'
                FROM CommodityTypes ct
                JOIN LookupData ld ON ct.NameId = ld.LookupDataId
                WHERE ld.LookupDataTextEnglish = 'Palm Oil'
                  AND ld.LookupDataTypeId = @NameType;
            ");
        }
    }
}
