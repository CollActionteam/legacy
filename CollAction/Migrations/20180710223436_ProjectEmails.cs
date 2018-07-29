using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class unsubscribetoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberProjectEmailsSend",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SubscribedToProjectEmails",
                table: "ProjectParticipants",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnsubscribeToken",
                table: "ProjectParticipants",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE public.\"ProjectParticipants\" SET \"UnsubscribeToken\"= uuid_in(md5(random()::text || now()::text || \"UserId\" || \"ProjectId\")::cstring);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnsubscribeToken",
                table: "ProjectParticipants");

            migrationBuilder.DropColumn(
                name: "NumberProjectEmailsSend",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "SubscribedToProjectEmails",
                table: "ProjectParticipants");
        }
    }
}
