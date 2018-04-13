using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CollAction.Migrations
{
    public partial class RemoveProjectsHtml : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorCommentsHtml",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DescriptionHtml",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "GoalHtml",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorCommentsHtml",
                table: "Projects",
                maxLength: 20000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionHtml",
                table: "Projects",
                maxLength: 10000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GoalHtml",
                table: "Projects",
                maxLength: 10000,
                nullable: false,
                defaultValue: "");
        }
    }
}
