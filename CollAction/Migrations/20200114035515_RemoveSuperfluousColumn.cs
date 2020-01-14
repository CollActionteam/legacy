using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class RemoveSuperfluousColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCategories_Projects_ProjectId1",
                table: "ProjectCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectCategories_ProjectId1",
                table: "ProjectCategories");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "ProjectCategories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId1",
                table: "ProjectCategories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_ProjectId1",
                table: "ProjectCategories",
                column: "ProjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCategories_Projects_ProjectId1",
                table: "ProjectCategories",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
