using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASBDDS.API.Migrations.DataDb
{
    public partial class UserAPIKeys_UPD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadOnly",
                table: "UserApiKeys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReadOnly",
                table: "UserApiKeys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
