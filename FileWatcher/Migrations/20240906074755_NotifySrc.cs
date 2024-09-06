using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class NotifySrc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifySourceTeamAutomatically",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifySourceTeamAutomatically",
                table: "Boxes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifySourceTeamAutomatically",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "NotifySourceTeamAutomatically",
                table: "Boxes");
        }
    }
}
