using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CollAction.Migrations
{
    public partial class NewDataProtection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataProtectionKeys",
                table: "DataProtectionKeys");

            migrationBuilder.AlterColumn<string>(
                name: "FriendlyName",
                table: "DataProtectionKeys",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(449)",
                oldMaxLength: 449);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DataProtectionKeys",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Xml",
                table: "DataProtectionKeys",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"public\".\"DataProtectionKeys\" SET \"Xml\" = \"KeyDataXml\"");

            migrationBuilder.DropColumn(
                name: "KeyDataXml",
                table: "DataProtectionKeys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataProtectionKeys",
                table: "DataProtectionKeys",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataProtectionKeys",
                table: "DataProtectionKeys");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DataProtectionKeys");

            migrationBuilder.AlterColumn<string>(
                name: "FriendlyName",
                table: "DataProtectionKeys",
                type: "character varying(449)",
                maxLength: 449,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyDataXml",
                table: "DataProtectionKeys",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"public\".\"DataProtectionKeys\" SET \"KeyDataXml\" = \"Xml\"");

            migrationBuilder.DropColumn(
                name: "Xml",
                table: "DataProtectionKeys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataProtectionKeys",
                table: "DataProtectionKeys",
                column: "FriendlyName");
        }
    }
}
