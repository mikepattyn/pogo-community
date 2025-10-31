using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gym.Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gyms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ControllingTeam = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsUnderAttack = table.Column<bool>(type: "boolean", nullable: false),
                    IsInRaid = table.Column<bool>(type: "boolean", nullable: false),
                    MotivationLevel = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gyms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_ControllingTeam",
                table: "Gyms",
                column: "ControllingTeam");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_CreatedAt",
                table: "Gyms",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_IsActive",
                table: "Gyms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_IsInRaid",
                table: "Gyms",
                column: "IsInRaid");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_IsUnderAttack",
                table: "Gyms",
                column: "IsUnderAttack");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_LastUpdated",
                table: "Gyms",
                column: "LastUpdated");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_LocationId",
                table: "Gyms",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_Name",
                table: "Gyms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Gyms_UpdatedAt",
                table: "Gyms",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gyms");
        }
    }
}
