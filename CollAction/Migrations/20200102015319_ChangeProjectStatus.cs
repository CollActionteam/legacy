using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class ChangeProjectStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE public.""Projects""
                SET ""Status"" = 2
                WHERE ""Status"" = 1 OR ""Status"" = 2 OR ""Status"" = 3;");
            migrationBuilder.Sql(@"
                UPDATE public.""Projects""
                SET ""Status"" = 1
                WHERE ""Status"" = 4;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE public.""Projects""
                SET ""Status"" = 4
                WHERE ""Status"" = 1;");
            migrationBuilder.Sql(@"
                UPDATE public.""Projects""
                SET ""Status"" = 1
                WHERE ""Status"" = 2;");
        }
    }
}
