using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectZero.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TvShow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReleasedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    Seasons = table.Column<int>(type: "integer", nullable: false),
                    Episodes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Director",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    TvShowId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Director", x => new { x.PersonId, x.TvShowId });
                    table.ForeignKey(
                        name: "FK_Director_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Director_TvShow_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Star",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    TvShowId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Star", x => new { x.PersonId, x.TvShowId });
                    table.ForeignKey(
                        name: "FK_Star_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Star_TvShow_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TvShowGenres",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "integer", nullable: false),
                    TvShowsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowGenres", x => new { x.GenresId, x.TvShowsId });
                    table.ForeignKey(
                        name: "FK_TvShowGenres_Genre_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowGenres_TvShow_TvShowsId",
                        column: x => x.TvShowsId,
                        principalTable: "TvShow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Writer",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    TvShowId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Writer", x => new { x.PersonId, x.TvShowId });
                    table.ForeignKey(
                        name: "FK_Writer_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Writer_TvShow_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TvShow",
                columns: new[] { "Id", "Episodes", "Name", "ReleasedAt", "Seasons" },
                values: new object[,]
                {
                    { 1, 62, "Breaking Bad", new DateOnly(2008, 1, 20), 5 },
                    { 2, 16, "The Last of Us", new DateOnly(2023, 1, 15), 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Director_TvShowId",
                table: "Director",
                column: "TvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Genre_Name",
                table: "Genre",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Star_TvShowId",
                table: "Star",
                column: "TvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowGenres_TvShowsId",
                table: "TvShowGenres",
                column: "TvShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_Writer_TvShowId",
                table: "Writer",
                column: "TvShowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Director");

            migrationBuilder.DropTable(
                name: "Star");

            migrationBuilder.DropTable(
                name: "TvShowGenres");

            migrationBuilder.DropTable(
                name: "Writer");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "TvShow");
        }
    }
}
