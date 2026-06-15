using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBothLocationTypesToDeliveryDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "DeliveryDetails",
                newName: "OriginLocationId");

            migrationBuilder.RenameColumn(
                name: "LeadTime",
                table: "DeliveryDetails",
                newName: "LeadTimeInDays");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationLocationId",
                table: "DeliveryDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_DestinationLocationId",
                table: "DeliveryDetails",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_OriginLocationId",
                table: "DeliveryDetails",
                column: "OriginLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetails_Locations_DestinationLocationId",
                table: "DeliveryDetails",
                column: "DestinationLocationId",
                principalTable: "Locations",
                principalColumn: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetails_Locations_OriginLocationId",
                table: "DeliveryDetails",
                column: "OriginLocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetails_Locations_DestinationLocationId",
                table: "DeliveryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetails_Locations_OriginLocationId",
                table: "DeliveryDetails");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDetails_DestinationLocationId",
                table: "DeliveryDetails");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDetails_OriginLocationId",
                table: "DeliveryDetails");

            migrationBuilder.DropColumn(
                name: "DestinationLocationId",
                table: "DeliveryDetails");

            migrationBuilder.RenameColumn(
                name: "OriginLocationId",
                table: "DeliveryDetails",
                newName: "LocationId");

            migrationBuilder.RenameColumn(
                name: "LeadTimeInDays",
                table: "DeliveryDetails",
                newName: "LeadTime");
        }
    }
}
