using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelGrandioso.Migrations;

/// <inheritdoc />
public partial class _20261122152416_AddSpa : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Treatments",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                Price = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Treatments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SpaBookings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                GuestId = table.Column<int>(type: "INTEGER", nullable: false),
                TreatmentId = table.Column<int>(type: "INTEGER", nullable: false),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SpaBookings", x => x.Id);
                table.ForeignKey(
                    name: "FK_SpaBookings_Guests_GuestId",
                    column: x => x.GuestId,
                    principalTable: "Guests",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SpaBookings_Treatments_TreatmentId",
                    column: x => x.TreatmentId,
                    principalTable: "Treatments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SpaBookings_GuestId",
            table: "SpaBookings",
            column: "GuestId");

        migrationBuilder.CreateIndex(
            name: "IX_SpaBookings_TreatmentId",
            table: "SpaBookings",
            column: "TreatmentId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SpaBookings");

        migrationBuilder.DropTable(
            name: "Treatments");
    }
}
