using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KarimDoors.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DoorPricingConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PricingProfileCode",
                table: "DoorTemplates",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuoteRoundingDigits",
                table: "DoorTemplates",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.Sql("""
                UPDATE DoorTemplates
                SET PricingProfileCode = CASE WHEN Code = 'HA-D04' THEN 'HA-2022' ELSE CONCAT('WB-', Code) END,
                    QuoteRoundingDigits = CASE WHEN Code LIKE 'RED-%' THEN 0 ELSE 2 END
                WHERE IsActive = 1 AND PricingProfileCode IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingProfileCode",
                table: "DoorTemplates");

            migrationBuilder.DropColumn(
                name: "QuoteRoundingDigits",
                table: "DoorTemplates");
        }
    }
}
