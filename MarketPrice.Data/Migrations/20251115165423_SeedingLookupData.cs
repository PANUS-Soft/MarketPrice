using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPrice.Migrations
{
    /// <inheritdoc />
    public partial class SeedingLookupData : Migration
    {
        /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Type 1000: Account Types ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (1001, 1000, 'Personal', 'Personal', N'Individuel')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (1002, 1000, 'Business', 'Business', N'Entreprise')");

            // --- Type 2000: Commodity Groups ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (2001, 2000, 'Grain', 'Grain', N'Graine')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (2002, 2000, 'Oil', 'Oil', N'Huile')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (2003, 2000, 'Legumes', 'Legumes', N'Légumineuse')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (2004, 2000, 'Vegetable', 'Vegetable', N'Légume')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (2005, 2000, 'Root', 'Root', N'Racine')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (2006, 2000, 'OilSeed', 'OilSeed', N'Oléagineux')");

            // --- Type 3000: Commodities ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (3001, 3000, 'Corn', 'Corn', N'Maïs')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (3002, 3000, 'PalmOil', 'Palm Oil', N'Huile de Palme')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (3003, 3000, 'Beans', 'Beans', N'Haricot')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (3004, 3000, 'Onion', 'Onion', N'Oignon')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (3005, 3000, 'Ginger', 'Ginger', N'Gingembre')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (3006, 3000, 'Egusi', 'Egusi', N'Pistcache')");

            // --- Type 4000: Address Types ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (4001, 4000, 'MainAddress', 'Main Address', N'Adresse Principale')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (4002, 4000, 'OtherAddress', 'Other Address', N'Autre Adresse')");

            // --- Type 5000: Position Statuses ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (5001, 5000, 'Open', 'Open', N'Ouvert')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (5002, 5000, 'Close', 'Close', N'Fermer')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (5003, 5000, 'Cancel', 'Cancel', N'Annuler')");

            // --- Type 6000: Position Types ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (6001, 6000, 'Bid', 'Bid', N'Offre')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (6002, 6000, 'Ask', 'Ask', N'Demande')");

            // --- Type 7000: Regions/Locations ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7001, 7000, 'Adamawa', 'Adamawa', N'Adamawa')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7002, 7000, 'Center', 'Center', N'Centre')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7003, 7000, 'East', 'East', N'Est')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7004, 7000, 'FarNorth', 'Far-North', N'Extrême-Nord')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7005, 7000, 'Littoral', 'Littoral', N'Littoral')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7006, 7000, 'North', 'North', N'Nord')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7007, 7000, 'NorthWest', 'Northwest', N'Nord-Ouest')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7008, 7000, 'South', 'South', N'Sud')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7009, 7000, 'SouthWest', 'Southwest', N'Sud-Ouest')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (7010, 7000, 'West', 'West', N'Ouest')");

            // --- Type 8000: Verification Statuses ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (8001, 8000, 'Pending', 'Pending', N'En cours')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (8002, 8000, 'Redo', 'Redo', N'Refaire')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (8003, 8000, 'Rejected', 'Rejected', N'Rejeté')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (8004, 8000, 'Unverified', 'Unverified', N'Non vérifiée')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (8005, 8000, 'Verified', 'Verified', N'Vérifiée')");

            // --- Type 9000: Verification Types ---
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (9001, 9000, 'Email', 'Email', N'E-mail')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (9002, 9000, 'IdCardNumber', 'Id Card Number', N'Numéro de carte d''identité')");
            migrationBuilder.Sql("INSERT INTO [LookupData] (LookupDataId, LookupDataTypeId, LookupDataValue, LookupDataTextEnglish, LookupDataTextFrench) VALUES (9003, 9000, 'PhoneNumber', 'Phone Number', N'Numéro de téléphone')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
