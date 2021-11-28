using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class OperationSystemsParams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstallationBootFile",
                table: "OperationSystemModels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstallationProtocol",
                table: "OperationSystemModels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OnlyInternalUsage",
                table: "OperationSystemModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Protocol",
                table: "OperationSystemModels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstallationBootFile",
                table: "OperationSystemModels");

            migrationBuilder.DropColumn(
                name: "InstallationProtocol",
                table: "OperationSystemModels");

            migrationBuilder.DropColumn(
                name: "OnlyInternalUsage",
                table: "OperationSystemModels");

            migrationBuilder.DropColumn(
                name: "Protocol",
                table: "OperationSystemModels");
        }
    }
}
