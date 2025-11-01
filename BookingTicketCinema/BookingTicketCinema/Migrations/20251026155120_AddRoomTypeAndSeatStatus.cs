using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicketCinema.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomTypeAndSeatStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Rooms");
        }
    }
}
