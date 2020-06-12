using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class InstagramUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVideoLink",
                table: "Crowdactions",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUser",
                table: "Crowdactions",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstagramUser",
                table: "Crowdactions");

            migrationBuilder.AlterColumn<string>(
                name: "DescriptionVideoLink",
                table: "Crowdactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2048,
                oldNullable: true);
        }
    }
}
