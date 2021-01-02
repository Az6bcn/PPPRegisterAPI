using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckinPPP.Migrations
{
    public partial class ShowSundayServiceFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowSundayService",
                table: "Bookings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowSundayService",
                table: "Bookings");
        }
    }
}
