using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingUnitOfMeasure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [UnitOfMeasures] (UnitOfMeasureId, UnitOfMeasureNameEnglish, UnitOfMeasureNameFrench, UnitOfMeasureCodeEnglish, UnitOfMeasureCodeFrench) " +
                                 "VALUES (NEWID(), 'Kilogram', 'Kilogramme', 'kg', 'kg')");
            migrationBuilder.Sql("INSERT INTO [UnitOfMeasures] (UnitOfMeasureId, UnitOfMeasureNameEnglish, UnitOfMeasureNameFrench, UnitOfMeasureCodeEnglish, UnitOfMeasureCodeFrench) " +
                                 "VALUES (NEWID(), 'Litre', 'Litre', 'L', 'L')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the seeded data if the migration is rolled back
            migrationBuilder.Sql("DELETE FROM [UnitOfMeasures] WHERE UnitOfMeasureCodeEnglish IN ('kg', 'L')");
        }
    }
}
