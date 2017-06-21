using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class ImageFileFilePathToHtmlFilepathNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Filepath",
                table: "ImageFiles",
                newName: "HtmlFilepath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HtmlFilepath",
                table: "ImageFiles",
                newName: "Filepath");
        }
    }
}
