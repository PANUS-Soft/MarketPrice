using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class NavigationPropertyToPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommodityId1",
                table: "Positions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStatusLookupDataId",
                table: "Positions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionTypeLookupDataId",
                table: "Positions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Positions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CommodityId1",
                table: "Positions",
                column: "CommodityId1");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CurrentStatusLookupDataId",
                table: "Positions",
                column: "CurrentStatusLookupDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionTypeLookupDataId",
                table: "Positions",
                column: "PositionTypeLookupDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_UserId1",
                table: "Positions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Commodities_CommodityId1",
                table: "Positions",
                column: "CommodityId1",
                principalTable: "Commodities",
                principalColumn: "CommodityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_LookupData_CurrentStatusLookupDataId",
                table: "Positions",
                column: "CurrentStatusLookupDataId",
                principalTable: "LookupData",
                principalColumn: "LookupDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_LookupData_PositionTypeLookupDataId",
                table: "Positions",
                column: "PositionTypeLookupDataId",
                principalTable: "LookupData",
                principalColumn: "LookupDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Users_UserId1",
                table: "Positions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Commodities_CommodityId1",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_LookupData_CurrentStatusLookupDataId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_LookupData_PositionTypeLookupDataId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Users_UserId1",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_CommodityId1",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_CurrentStatusLookupDataId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_PositionTypeLookupDataId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_UserId1",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "CommodityId1",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "CurrentStatusLookupDataId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "PositionTypeLookupDataId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Positions");
        }
    }
}
