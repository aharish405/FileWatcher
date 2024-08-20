using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class excludeCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExcludeCalendarId",
                table: "Boxes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExcludeCalendar",
                columns: table => new
                {
                    ExcludeCalendarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExcludedDates = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExcludeCalendar", x => x.ExcludeCalendarId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_ExcludeCalendarId",
                table: "Boxes",
                column: "ExcludeCalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_ExcludeCalendar_ExcludeCalendarId",
                table: "Boxes",
                column: "ExcludeCalendarId",
                principalTable: "ExcludeCalendar",
                principalColumn: "ExcludeCalendarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_ExcludeCalendar_ExcludeCalendarId",
                table: "Boxes");

            migrationBuilder.DropTable(
                name: "ExcludeCalendar");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_ExcludeCalendarId",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "ExcludeCalendarId",
                table: "Boxes");
        }
    }
}
