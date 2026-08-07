using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Migrations
{
    /// <inheritdoc />
    public partial class SeedingLookupDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (1000, 'AccountTypes')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (2000, 'CommodityGroups')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (3000, 'CommodityTypes')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (4000, 'LocationTypes')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (5000, 'PositionCurrentStatus')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (6000, 'PositionTypes')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (7000, 'Regions')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (8000, 'VerificationCurrentStatus')");
            migrationBuilder.Sql("INSERT INTO [LookupDataTypes] (LookupDataTypeId, LookupDataTypeName) VALUES (9000, 'VerificationTypes')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
