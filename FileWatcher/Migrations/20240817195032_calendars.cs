using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class calendars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalendarId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreBoxSchedule",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CalendarId",
                table: "Boxes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalendarId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarDay_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CalendarId",
                table: "Jobs",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_CalendarId",
                table: "Boxes",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarDay_CalendarId",
                table: "CalendarDay",
                column: "CalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_Calendars_CalendarId",
                table: "Boxes",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Calendars_CalendarId",
                table: "Jobs",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_Calendars_CalendarId",
                table: "Boxes");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Calendars_CalendarId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "CalendarDay");

            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CalendarId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Boxes_CalendarId",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IgnoreBoxSchedule",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CalendarId",
                table: "Boxes");
        }
    }
}
