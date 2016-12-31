using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class ProjectTweaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayPriority",
                table: "Projects",
                nullable: false,
                defaultValue: 1)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayPriority",
                table: "Projects");
        }
    }
}
