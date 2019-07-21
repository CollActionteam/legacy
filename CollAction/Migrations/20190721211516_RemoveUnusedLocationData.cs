using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CollAction.Migrations
{
    public partial class RemoveUnusedLocationData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Locations_LocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Locations_LocationId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationContinents_Locations_LocationId",
                table: "LocationContinents");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationCountries_Locations_LocationId",
                table: "LocationCountries");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationLevel1_Locations_LocationId",
                table: "LocationLevel1");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationLevel2_Locations_LocationId",
                table: "LocationLevel2");

            migrationBuilder.DropTable(
                name: "LocationAlternateNames");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "LocationCountries");

            migrationBuilder.DropTable(
                name: "LocationLevel1");

            migrationBuilder.DropTable(
                name: "LocationLevel2");

            migrationBuilder.DropTable(
                name: "LocationContinents");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LocationId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_LocationId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Jobs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationCountries",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 2, nullable: false),
                    CapitalCity = table.Column<string>(maxLength: 200, nullable: false),
                    ContinentId = table.Column<string>(maxLength: 2, nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CountryId = table.Column<string>(maxLength: 2, nullable: true),
                    Feature = table.Column<int>(nullable: false),
                    FeatureClass = table.Column<int>(nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(13,10)", nullable: false),
                    Level1Id = table.Column<string>(maxLength: 20, nullable: true),
                    Level2Id = table.Column<string>(maxLength: 80, nullable: true),
                    LocationContinentId = table.Column<string>(nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(13,10)", nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    TimeZone = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_LocationCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "LocationCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LocationAlternateNames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AlternateName = table.Column<string>(maxLength: 400, nullable: false),
                    IsColloquial = table.Column<bool>(nullable: false),
                    IsHistoric = table.Column<bool>(nullable: false),
                    IsPreferredName = table.Column<bool>(nullable: false),
                    IsShortName = table.Column<bool>(nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 7, nullable: false),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationAlternateNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationAlternateNames_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationContinents",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 2, nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationContinents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationContinents_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationLevel1",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 20, nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationLevel1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationLevel1_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationLevel2",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 80, nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationLevel2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationLevel2_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LocationId",
                table: "Projects",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_LocationId",
                table: "Jobs",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationAlternateNames_LocationId",
                table: "LocationAlternateNames",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationContinents_LocationId",
                table: "LocationContinents",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationCountries_ContinentId",
                table: "LocationCountries",
                column: "ContinentId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationCountries_LocationId",
                table: "LocationCountries",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLevel1_LocationId",
                table: "LocationLevel1",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLevel2_LocationId",
                table: "LocationLevel2",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CountryId",
                table: "Locations",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Level1Id",
                table: "Locations",
                column: "Level1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Level2Id",
                table: "Locations",
                column: "Level2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_LocationContinentId",
                table: "Locations",
                column: "LocationContinentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Locations_LocationId",
                table: "Jobs",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Locations_LocationId",
                table: "Projects",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationCountries_Locations_LocationId",
                table: "LocationCountries",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationCountries_LocationContinents_ContinentId",
                table: "LocationCountries",
                column: "ContinentId",
                principalTable: "LocationContinents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_LocationContinents_LocationContinentId",
                table: "Locations",
                column: "LocationContinentId",
                principalTable: "LocationContinents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_LocationLevel1_Level1Id",
                table: "Locations",
                column: "Level1Id",
                principalTable: "LocationLevel1",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_LocationLevel2_Level2Id",
                table: "Locations",
                column: "Level2Id",
                principalTable: "LocationLevel2",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
