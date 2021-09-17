using Microsoft.EntityFrameworkCore.Migrations;

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class AddDevicePowerControlType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PowerControlType",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PowerControlType",
                table: "Devices");
        }
    }
}
