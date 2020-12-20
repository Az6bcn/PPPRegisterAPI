using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckinPPP.Migrations
{
    public partial class SpecialServiceYoutubeUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecialServiceYoutubeUrl",
                table: "Bookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialServiceYoutubeUrl",
                table: "Bookings");
        }
    }
}
