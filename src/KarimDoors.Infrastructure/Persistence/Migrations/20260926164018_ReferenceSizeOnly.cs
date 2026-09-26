using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KarimDoors.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceSizeOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReferenceSizeOnly",
                table: "DoorTemplateVersions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceSizeOnly",
                table: "DoorTemplateVersions");
        }
    }
}
