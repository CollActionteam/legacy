using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Data.Migrations
{
    public partial class Required : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Project",
                maxLength: 256,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Project",
                maxLength: 1024,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Project",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Project",
                maxLength: 128,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Project",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Project",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Project",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Project",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
