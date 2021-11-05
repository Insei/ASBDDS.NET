using Microsoft.EntityFrameworkCore.Migrations;

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class AddDeviceMachineState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StateEnum",
                table: "Devices",
                newName: "PowerState");

            migrationBuilder.AddColumn<int>(
                name: "MachineState",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MachineState",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "PowerState",
                table: "Devices",
                newName: "StateEnum");
        }
    }
}
