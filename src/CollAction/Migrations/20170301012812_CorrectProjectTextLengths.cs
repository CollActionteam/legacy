using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class CorrectProjectTextLengths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Proposal",
                table: "Projects",
                maxLength: 300,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                maxLength: 50,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "Projects",
                maxLength: 1000,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                maxLength: 1000,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorComments",
                table: "Projects",
                maxLength: 2000,
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Proposal",
                table: "Projects",
                maxLength: 512,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                maxLength: 128,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "Projects",
                maxLength: 1024,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                maxLength: 1024,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorComments",
                table: "Projects",
                maxLength: 2048,
                nullable: false);
        }
    }
}
