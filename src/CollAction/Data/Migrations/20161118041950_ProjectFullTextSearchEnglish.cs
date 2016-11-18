using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Data.Migrations
{
    public partial class ProjectFullTextSearchEnglish : Migration
    {
        private string language = "english";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string ftsvColumnName = "FullTextSearchVector_" + language;

            // Add the FullTextSearchVector column.
            migrationBuilder.Sql("ALTER TABLE \"Project\" ADD COLUMN \"" + ftsvColumnName + "\" tsvector;");

            // Update existing data to have a valid tsv.
            migrationBuilder.Sql("UPDATE \"Project\" SET \"" + ftsvColumnName + "\" = " + GetWeightedTsvector(language) + ";");

            // Create a gist index on the FullTextSearchVector column for fast fts lookups.
            // N.B. using gist rather than gin as it allows weighted indexes.
            migrationBuilder.Sql("CREATE INDEX \"IX_Project_" + ftsvColumnName + "\" ON \"Project\" USING gist(\"" + ftsvColumnName + "\");");

            // Create a function to update the FullTextSearchVector tsvector column on the Project table.
            migrationBuilder.Sql("CREATE FUNCTION Project_Update_" + ftsvColumnName + "() RETURNS trigger AS $$ " +
                "begin " +
                    "new.\"" + ftsvColumnName + "\" := " + GetWeightedTsvector(language, "new.") + "; " +
                    "return new; " +
                "end " +
                "$$ LANGUAGE plpgsql;");

            migrationBuilder.Sql(
                "CREATE TRIGGER \"" + ftsvColumnName + "Update\" BEFORE INSERT OR UPDATE ON \"Project\" " +
                "FOR EACH ROW EXECUTE PROCEDURE Project_Update_" + ftsvColumnName + "();");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string ftsvColumnName = "FullTextSearchVector_" + language;

            migrationBuilder.Sql("DROP TRIGGER \"" + ftsvColumnName + "Update\" ON \"Project\";");

            migrationBuilder.Sql("DROP FUNCTION Project_Update_" + ftsvColumnName + "();");

            migrationBuilder.DropIndex("IX_Project_" + ftsvColumnName, "Project");

            migrationBuilder.Sql("ALTER TABLE \"Project\" DROP COLUMN \"" + ftsvColumnName + "\";");
        }

        private string GetWeightedTsvector(string language, string entityName = "")
        {
            return "setweight(to_tsvector('" + language + "', coalesce(" + entityName + "\"Title\", '')), 'A') || " +
                        "setweight(to_tsvector('" + language + "', coalesce(" + entityName + "\"ShortDescription\", '')), 'B') || " +
                        "setweight(to_tsvector('" + language + "', coalesce(" + entityName + "\"Description\", '')), 'C')";
        }
    }
}
