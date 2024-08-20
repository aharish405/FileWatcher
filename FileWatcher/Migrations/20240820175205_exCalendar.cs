using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class exCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_ExcludeCalendar_ExcludeCalendarId",
                table: "Boxes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcludeCalendar",
                table: "ExcludeCalendar");

            migrationBuilder.RenameTable(
                name: "ExcludeCalendar",
                newName: "ExcludeCalendars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcludeCalendars",
                table: "ExcludeCalendars",
                column: "ExcludeCalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_ExcludeCalendars_ExcludeCalendarId",
                table: "Boxes",
                column: "ExcludeCalendarId",
                principalTable: "ExcludeCalendars",
                principalColumn: "ExcludeCalendarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boxes_ExcludeCalendars_ExcludeCalendarId",
                table: "Boxes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcludeCalendars",
                table: "ExcludeCalendars");

            migrationBuilder.RenameTable(
                name: "ExcludeCalendars",
                newName: "ExcludeCalendar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcludeCalendar",
                table: "ExcludeCalendar",
                column: "ExcludeCalendarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boxes_ExcludeCalendar_ExcludeCalendarId",
                table: "Boxes",
                column: "ExcludeCalendarId",
                principalTable: "ExcludeCalendar",
                principalColumn: "ExcludeCalendarId");
        }
    }
}
