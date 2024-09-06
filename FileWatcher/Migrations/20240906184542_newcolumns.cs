using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileWatcherApp.Migrations
{
    /// <inheritdoc />
    public partial class newcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AxwayID",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AxwayID",
                table: "Jobs");
        }
    }
}
