using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AggregatedPrices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommodityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Interval = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AvgBid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HighBid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LowBid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgOffer = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HighOffer = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LowOffer = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PositionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LookupDataTypes",
                columns: table => new
                {
                    LookupDataTypeId = table.Column<int>(type: "int", nullable: false),
                    LookupDataTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupDataTypes", x => x.LookupDataTypeId);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasures",
                columns: table => new
                {
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UnitOfMeasureNameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureNameFrench = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureCodeEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureCodeFrench = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasures", x => x.UnitOfMeasureId);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurityDetails",
                columns: table => new
                {
                    SecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityDetails", x => x.SecurityId);
                });

            migrationBuilder.CreateTable(
                name: "LookupData",
                columns: table => new
                {
                    LookupDataId = table.Column<int>(type: "int", nullable: false),
                    LookupDataTypeId = table.Column<int>(type: "int", nullable: false),
                    LookupDataValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LookupDataTextEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LookupDataTextFrench = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupData", x => x.LookupDataId);
                    table.ForeignKey(
                        name: "FK_LookupData_LookupDataTypes_LookupDataTypeId",
                        column: x => x.LookupDataTypeId,
                        principalTable: "LookupDataTypes",
                        principalColumn: "LookupDataTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commodities",
                columns: table => new
                {
                    CommodityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CommodityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommodityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShelfLifeInDays = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotSize = table.Column<short>(type: "smallint", nullable: true),
                    PreviousBestBid = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LastBestBid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviousBestOffer = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LastBestOffer = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsBidImproved = table.Column<bool>(type: "bit", nullable: false),
                    IsOfferImproved = table.Column<bool>(type: "bit", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodities", x => x.CommodityId);
                    table.ForeignKey(
                        name: "FK_Commodities_UnitOfMeasures_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "UnitOfMeasureId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommodityTypes",
                columns: table => new
                {
                    CommodityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CommodityGroupId = table.Column<int>(type: "int", nullable: false),
                    NameId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultUnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastBestBid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LastBestOffer = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsBidImproved = table.Column<bool>(type: "bit", nullable: false),
                    IsOfferImproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityTypes", x => x.CommodityTypeId);
                    table.ForeignKey(
                        name: "FK_CommodityTypes_LookupData_CommodityGroupId",
                        column: x => x.CommodityGroupId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_CommodityTypes_LookupData_NameId",
                        column: x => x.NameId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_CommodityTypes_UnitOfMeasures_DefaultUnitOfMeasureId",
                        column: x => x.DefaultUnitOfMeasureId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "UnitOfMeasureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AccountTypeId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCardNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPremiumUser = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateRecorded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUpdate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_LookupData_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                });

            migrationBuilder.CreateTable(
                name: "CommodityImage",
                columns: table => new
                {
                    CommodityImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommodityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityImage", x => x.CommodityImageId);
                    table.ForeignKey(
                        name: "FK_CommodityImage_Commodities_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodities",
                        principalColumn: "CommodityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommodityTypeImage",
                columns: table => new
                {
                    CommodityTypeImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommodityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityTypeImage", x => x.CommodityTypeImageId);
                    table.ForeignKey(
                        name: "FK_CommodityTypeImage_CommodityTypes_CommodityTypeId",
                        column: x => x.CommodityTypeId,
                        principalTable: "CommodityTypes",
                        principalColumn: "CommodityTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    LocationTypeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quarter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_LookupData_LocationTypeId",
                        column: x => x.LocationTypeId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_Locations_LookupData_RegionId",
                        column: x => x.RegionId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_Locations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommodityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionTypeId = table.Column<int>(type: "int", nullable: false),
                    CurrentStatusId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_Positions_Commodities_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodities",
                        principalColumn: "CommodityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Positions_LookupData_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_Positions_LookupData_PositionTypeId",
                        column: x => x.PositionTypeId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_Positions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RaterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateRecorded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateUpdated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.CheckConstraint("CHK_Ratings_Score", "[Score] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Ratings_Users_RatedUserId",
                        column: x => x.RatedUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_RaterUserId",
                        column: x => x.RaterUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Verifications",
                columns: table => new
                {
                    VerificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationTypeId = table.Column<int>(type: "int", nullable: false),
                    CurrentVerificationStatusId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateStarted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateCompleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.VerificationId);
                    table.ForeignKey(
                        name: "FK_Verifications_LookupData_CurrentVerificationStatusId",
                        column: x => x.CurrentVerificationStatusId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_Verifications_LookupData_VerificationTypeId",
                        column: x => x.VerificationTypeId,
                        principalTable: "LookupData",
                        principalColumn: "LookupDataId");
                    table.ForeignKey(
                        name: "FK_Verifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDetails",
                columns: table => new
                {
                    DeliveryDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeliverable = table.Column<bool>(type: "bit", nullable: false),
                    LeadTimeInDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxDistance = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDetails", x => x.DeliveryDetailId);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId");
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_Locations_OriginLocationId",
                        column: x => x.OriginLocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedPrices_Lookup",
                table: "AggregatedPrices",
                columns: new[] { "CommodityId", "Interval", "Timestamp" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commodities_UnitOfMeasureId",
                table: "Commodities",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityImage_CommodityId",
                table: "CommodityImage",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTypeImage_CommodityTypeId",
                table: "CommodityTypeImage",
                column: "CommodityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTypes_CommodityGroupId",
                table: "CommodityTypes",
                column: "CommodityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTypes_DefaultUnitOfMeasureId",
                table: "CommodityTypes",
                column: "DefaultUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityTypes_NameId",
                table: "CommodityTypes",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_DestinationLocationId",
                table: "DeliveryDetails",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_OriginLocationId",
                table: "DeliveryDetails",
                column: "OriginLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_PositionId",
                table: "DeliveryDetails",
                column: "PositionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_LocationTypeId",
                table: "Locations",
                column: "LocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RegionId",
                table: "Locations",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupData_LookupDataTypeId",
                table: "LookupData",
                column: "LookupDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CommodityId",
                table: "Positions",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CurrentStatusId",
                table: "Positions",
                column: "CurrentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionTypeId",
                table: "Positions",
                column: "PositionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_UserId",
                table: "Positions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RatedUserId_RaterUserId",
                table: "Ratings",
                columns: new[] { "RatedUserId", "RaterUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RaterUserId",
                table: "Ratings",
                column: "RaterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountTypeId",
                table: "Users",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdCardNumber",
                table: "Users",
                column: "IdCardNumber",
                unique: true,
                filter: "[IdCardNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurityDetails_UserId",
                table: "UserSecurityDetails",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_CurrentVerificationStatusId",
                table: "Verifications",
                column: "CurrentVerificationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_UserId",
                table: "Verifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_VerificationTypeId",
                table: "Verifications",
                column: "VerificationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedPrices");

            migrationBuilder.DropTable(
                name: "CommodityImage");

            migrationBuilder.DropTable(
                name: "CommodityTypeImage");

            migrationBuilder.DropTable(
                name: "DeliveryDetails");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "UserSecurityDetails");

            migrationBuilder.DropTable(
                name: "Verifications");

            migrationBuilder.DropTable(
                name: "CommodityTypes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Commodities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UnitOfMeasures");

            migrationBuilder.DropTable(
                name: "LookupData");

            migrationBuilder.DropTable(
                name: "LookupDataTypes");
        }
    }
}
