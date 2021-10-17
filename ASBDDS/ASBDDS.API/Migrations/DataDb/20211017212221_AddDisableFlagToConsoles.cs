using Microsoft.EntityFrameworkCore.Migrations;

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class AddDisableFlagToConsoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "Consoles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "Consoles");
        }
    }
}
