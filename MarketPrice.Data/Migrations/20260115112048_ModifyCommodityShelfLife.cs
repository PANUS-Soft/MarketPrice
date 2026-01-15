using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyCommodityShelfLife : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShelfLifeInDays",
                table: "Commodities",
                type: "int",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "ShelfLifeInDays",
                table: "Commodities",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
