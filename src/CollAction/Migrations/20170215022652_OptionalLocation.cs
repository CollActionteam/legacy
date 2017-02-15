using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class OptionalLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Locations_LocationId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Posting",
                table: "Jobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostDate",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Locations_LocationId",
                table: "Jobs",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Locations_LocationId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PostDate",
                table: "Jobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "Posting",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Jobs",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Locations_LocationId",
                table: "Jobs",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
