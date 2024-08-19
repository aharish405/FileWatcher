using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class removeUnNecessaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ScheduleDays",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Boxes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleDays",
                table: "Boxes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Boxes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
