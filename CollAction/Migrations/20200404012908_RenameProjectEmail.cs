using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class RenameProjectEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberProjectEmailsSend",
                newName: "NumberProjectEmailsSent",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberProjectEmailsSent",
                newName: "NumberProjectEmailsSend",
                table: "Projects");
        }
    }
}
