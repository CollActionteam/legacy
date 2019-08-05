using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class Descriptionimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescriptiveImageFileId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ImageFiles",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DescriptiveImageFileId",
                table: "Projects",
                column: "DescriptiveImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ImageFiles_DescriptiveImageFileId",
                table: "Projects",
                column: "DescriptiveImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ImageFiles_DescriptiveImageFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DescriptiveImageFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DescriptiveImageFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ImageFiles");
        }
    }
}
