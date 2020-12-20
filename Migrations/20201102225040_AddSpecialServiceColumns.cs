using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckinPPP.Migrations
{
    public partial class AddSpecialServiceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialService",
                table: "Bookings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpecialServiceName",
                table: "Bookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpecialService",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SpecialServiceName",
                table: "Bookings");
        }
    }
}
