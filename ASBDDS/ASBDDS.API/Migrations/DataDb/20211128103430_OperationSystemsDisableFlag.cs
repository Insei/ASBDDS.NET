using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class OperationSystemsDisableFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "OperationSystemModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "OperationSystemModels");
        }
    }
}
