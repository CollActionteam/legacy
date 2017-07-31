using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class CA_256_change_category_names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Categories\" SET \"Description\" = 'Consumption', \"Name\" = 'Consumption' WHERE \"Name\" = 'Consuming'");
            migrationBuilder.Sql("UPDATE \"Categories\" SET \"Description\" = 'Well-being', \"Name\" = 'Well-being' WHERE \"Name\" = 'Wellbeing'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Categories\" SET \"Description\" = 'Consuming', \"Name\" = 'Consuming' WHERE \"Name\" = 'Consumption'");
            migrationBuilder.Sql("UPDATE \"Categories\" SET \"Description\" = 'Wellbeing', \"Name\" = 'Wellbeing' WHERE \"Name\" = 'Well-being'");
        }
    }
}
