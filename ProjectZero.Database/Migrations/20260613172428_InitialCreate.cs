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
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TvShows",
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
                    table.PrimaryKey("PK_TvShows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Writers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Writers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TvShowDirectors",
                columns: table => new
                {
                    DirectorsId = table.Column<int>(type: "integer", nullable: false),
                    TvShowsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowDirectors", x => new { x.DirectorsId, x.TvShowsId });
                    table.ForeignKey(
                        name: "FK_TvShowDirectors_Directors_DirectorsId",
                        column: x => x.DirectorsId,
                        principalTable: "Directors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowDirectors_TvShows_TvShowsId",
                        column: x => x.TvShowsId,
                        principalTable: "TvShows",
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
                        name: "FK_TvShowGenres_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowGenres_TvShows_TvShowsId",
                        column: x => x.TvShowsId,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TvShowStars",
                columns: table => new
                {
                    StarsId = table.Column<int>(type: "integer", nullable: false),
                    TvShowsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowStars", x => new { x.StarsId, x.TvShowsId });
                    table.ForeignKey(
                        name: "FK_TvShowStars_Stars_StarsId",
                        column: x => x.StarsId,
                        principalTable: "Stars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowStars_TvShows_TvShowsId",
                        column: x => x.TvShowsId,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TvShowWriters",
                columns: table => new
                {
                    TvShowsId = table.Column<int>(type: "integer", nullable: false),
                    WritersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowWriters", x => new { x.TvShowsId, x.WritersId });
                    table.ForeignKey(
                        name: "FK_TvShowWriters_TvShows_TvShowsId",
                        column: x => x.TvShowsId,
                        principalTable: "TvShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowWriters_Writers_WritersId",
                        column: x => x.WritersId,
                        principalTable: "Writers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TvShows",
                columns: new[] { "Id", "Episodes", "Name", "ReleasedAt", "Seasons" },
                values: new object[,]
                {
                    { 1, 62, "Breaking Bad", new DateOnly(2008, 1, 20), 5 },
                    { 2, 16, "The Last of Us", new DateOnly(2023, 1, 15), 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TvShowDirectors_TvShowsId",
                table: "TvShowDirectors",
                column: "TvShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowGenres_TvShowsId",
                table: "TvShowGenres",
                column: "TvShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowStars_TvShowsId",
                table: "TvShowStars",
                column: "TvShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowWriters_WritersId",
                table: "TvShowWriters",
                column: "WritersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TvShowDirectors");

            migrationBuilder.DropTable(
                name: "TvShowGenres");

            migrationBuilder.DropTable(
                name: "TvShowStars");

            migrationBuilder.DropTable(
                name: "TvShowWriters");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Stars");

            migrationBuilder.DropTable(
                name: "TvShows");

            migrationBuilder.DropTable(
                name: "Writers");
        }
    }
}
