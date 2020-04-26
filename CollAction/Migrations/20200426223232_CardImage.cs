using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class CardImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardImageFileId",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CardImageFileId",
                table: "Projects",
                column: "CardImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ImageFiles_CardImageFileId",
                table: "Projects",
                column: "CardImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ImageFiles_CardImageFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CardImageFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CardImageFileId",
                table: "Projects");
        }
    }
}
