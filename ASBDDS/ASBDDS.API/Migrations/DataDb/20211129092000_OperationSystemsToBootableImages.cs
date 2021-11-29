using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class OperationSystemsToBootableImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedOsFiles");

            migrationBuilder.DropTable(
                name: "OperationSystemModels");

            migrationBuilder.CreateTable(
                name: "BootableImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    Arch = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    InProtocol = table.Column<int>(type: "integer", nullable: false),
                    BootFile = table.Column<string>(type: "text", nullable: true),
                    OutProtocol = table.Column<int>(type: "integer", nullable: false),
                    Options = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BootableImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharedBootableImageFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BootableImageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ShareViaHttp = table.Column<bool>(type: "boolean", nullable: false),
                    ShareViaTftp = table.Column<bool>(type: "boolean", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedBootableImageFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedBootableImageFiles_BootableImages_BootableImageId",
                        column: x => x.BootableImageId,
                        principalTable: "BootableImages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedBootableImageFiles_BootableImageId",
                table: "SharedBootableImageFiles",
                column: "BootableImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharedBootableImageFiles");

            migrationBuilder.DropTable(
                name: "BootableImages");

            migrationBuilder.CreateTable(
                name: "OperationSystemModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Arch = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false),
                    InstallationBootFile = table.Column<string>(type: "text", nullable: true),
                    InstallationProtocol = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    OnlyInternalUsage = table.Column<bool>(type: "boolean", nullable: false),
                    Options = table.Column<string>(type: "text", nullable: true),
                    Protocol = table.Column<int>(type: "integer", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true)
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
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShareViaHttp = table.Column<bool>(type: "boolean", nullable: false),
                    ShareViaTftp = table.Column<bool>(type: "boolean", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
    }
}
