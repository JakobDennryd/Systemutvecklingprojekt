using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weather.Report.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "weather_report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AverageHighC = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageLowC = table.Column<decimal>(type: "numeric", nullable: false),
                    RainfallTotalCm = table.Column<decimal>(type: "numeric", nullable: false),
                    SnowTotalCm = table.Column<decimal>(type: "numeric", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weather_report", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weather_report");
        }
    }
}
