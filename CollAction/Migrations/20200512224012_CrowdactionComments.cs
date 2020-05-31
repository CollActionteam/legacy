using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CollAction.Migrations
{
    public partial class CrowdactionComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrowdactionComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommentedAt = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CrowdactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrowdactionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrowdactionComments_Crowdactions_CrowdactionId",
                        column: x => x.CrowdactionId,
                        principalTable: "Crowdactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrowdactionComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrowdactionComments_CrowdactionId",
                table: "CrowdactionComments",
                column: "CrowdactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CrowdactionComments_UserId",
                table: "CrowdactionComments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrowdactionComments");
        }
    }
}
