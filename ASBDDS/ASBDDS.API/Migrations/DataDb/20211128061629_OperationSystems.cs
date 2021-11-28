using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class OperationSystems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationSystemModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    Arch = table.Column<string>(type: "text", nullable: true),
                    Options = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationSystemModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharedOsFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OsId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ShareViaHttp = table.Column<bool>(type: "boolean", nullable: false),
                    ShareViaTftp = table.Column<bool>(type: "boolean", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedOsFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedOsFiles_OperationSystemModels_OsId",
                        column: x => x.OsId,
                        principalTable: "OperationSystemModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedOsFiles_OsId",
                table: "SharedOsFiles",
                column: "OsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedOsFiles");

            migrationBuilder.DropTable(
                name: "OperationSystemModels");
        }
    }
}
