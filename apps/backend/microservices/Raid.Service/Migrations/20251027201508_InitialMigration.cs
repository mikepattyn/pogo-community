using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Raid.Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Raids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordMessageId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GymId = table.Column<int>(type: "integer", nullable: false),
                    PokemonSpecies = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    CurrentParticipants = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WeatherBoost = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raids", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Raids_CreatedAt",
                table: "Raids",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_Difficulty",
                table: "Raids",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_DiscordMessageId",
                table: "Raids",
                column: "DiscordMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Raids_EndTime",
                table: "Raids",
                column: "EndTime");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_GymId",
                table: "Raids",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_GymId_StartTime",
                table: "Raids",
                columns: new[] { "GymId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Raids_IsActive",
                table: "Raids",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_IsActive_StartTime",
                table: "Raids",
                columns: new[] { "IsActive", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Raids_IsCancelled",
                table: "Raids",
                column: "IsCancelled");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_IsCompleted",
                table: "Raids",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_Level",
                table: "Raids",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_Level_IsActive",
                table: "Raids",
                columns: new[] { "Level", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Raids_PokemonSpecies",
                table: "Raids",
                column: "PokemonSpecies");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_StartTime",
                table: "Raids",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Raids_UpdatedAt",
                table: "Raids",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Raids");
        }
    }
}
