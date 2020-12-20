using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckinPPP.Migrations
{
    public partial class SpecialServiceAnnouncement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShowSpecialAnnouncement",
                table: "Bookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowSpecialAnnouncement",
                table: "Bookings");
        }
    }
}
