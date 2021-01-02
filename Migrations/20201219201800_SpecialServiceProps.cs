using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckinPPP.Migrations
{
    public partial class SpecialServiceProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowSpecialService",
                table: "Bookings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowSpecialServiceSlotDetails",
                table: "Bookings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowSpecialService",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ShowSpecialServiceSlotDetails",
                table: "Bookings");
        }
    }
}
