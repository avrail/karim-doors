using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KarimDoors.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DoorTemplateProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "DoorTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoorTemplates_ProjectId",
                table: "DoorTemplates",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoorTemplates_Projects_ProjectId",
                table: "DoorTemplates",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoorTemplates_Projects_ProjectId",
                table: "DoorTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DoorTemplates_ProjectId",
                table: "DoorTemplates");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "DoorTemplates");
        }
    }
}
