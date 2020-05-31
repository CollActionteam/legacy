using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class FinishJobId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinishJobId",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishJobId",
                table: "Projects");
        }
    }
}
