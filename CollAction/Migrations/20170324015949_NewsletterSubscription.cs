using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollAction.Migrations
{
    public partial class NewsletterSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewsletterSubscriptionId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NewsletterSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NewsletterSubscriptionId",
                table: "AspNetUsers",
                column: "NewsletterSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterSubscription_Email",
                table: "NewsletterSubscriptions",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_NewsletterSubscriptions_NewsletterSubscriptionId",
                table: "AspNetUsers",
                column: "NewsletterSubscriptionId",
                principalTable: "NewsletterSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_NewsletterSubscriptions_NewsletterSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "NewsletterSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_NewsletterSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NewsletterSubscriptionId",
                table: "AspNetUsers");
        }
    }
}
