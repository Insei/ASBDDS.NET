using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class AddConsoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConsoleId",
                table: "Devices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SerialPortsSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PortName = table.Column<string>(type: "text", nullable: true),
                    BaudRate = table.Column<int>(type: "integer", nullable: false),
                    DataBits = table.Column<int>(type: "integer", nullable: false),
                    StopBits = table.Column<int>(type: "integer", nullable: false),
                    Parity = table.Column<int>(type: "integer", nullable: false),
                    Handshake = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialPortsSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SerialSettingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consoles_SerialPortsSettings_SerialSettingsId",
                        column: x => x.SerialSettingsId,
                        principalTable: "SerialPortsSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ConsoleId",
                table: "Devices",
                column: "ConsoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Consoles_SerialSettingsId",
                table: "Consoles",
                column: "SerialSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Consoles_ConsoleId",
                table: "Devices",
                column: "ConsoleId",
                principalTable: "Consoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Consoles_ConsoleId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "Consoles");

            migrationBuilder.DropTable(
                name: "SerialPortsSettings");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ConsoleId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ConsoleId",
                table: "Devices");
        }
    }
}
