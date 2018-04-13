using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CollAction.Migrations
{
    public partial class UpdateEndDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE public.\"Projects\" SET \"End\" = \"End\" + interval '23 hours' + interval '59 minutes' + interval '59 seconds';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE public.\"Projects\" SET \"End\" = \"End\" - interval '23 hours' - interval '59 minutes' - interval '59 seconds';");
        }
    }
}
