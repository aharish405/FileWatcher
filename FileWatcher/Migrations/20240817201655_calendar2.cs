using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class calendar2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarDay_Calendars_CalendarId",
                table: "CalendarDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarDay",
                table: "CalendarDay");

            migrationBuilder.RenameTable(
                name: "CalendarDay",
                newName: "CalendarDays");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarDay_CalendarId",
                table: "CalendarDays",
                newName: "IX_CalendarDays_CalendarId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Calendars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Calendars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarDays",
                table: "CalendarDays",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarDays_Calendars_CalendarId",
                table: "CalendarDays",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarDays_Calendars_CalendarId",
                table: "CalendarDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarDays",
                table: "CalendarDays");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Calendars");

            migrationBuilder.RenameTable(
                name: "CalendarDays",
                newName: "CalendarDay");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarDays_CalendarId",
                table: "CalendarDay",
                newName: "IX_CalendarDay_CalendarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarDay",
                table: "CalendarDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarDay_Calendars_CalendarId",
                table: "CalendarDay",
                column: "CalendarId",
                principalTable: "Calendars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
