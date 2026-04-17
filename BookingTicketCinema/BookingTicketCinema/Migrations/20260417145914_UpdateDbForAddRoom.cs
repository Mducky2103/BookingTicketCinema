using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicketCinema.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbForAddRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColumnIndex",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RowIndex",
                table: "Seats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "SeatGroups",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalColumns",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRows",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnIndex",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "RowIndex",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "SeatGroups");

            migrationBuilder.DropColumn(
                name: "TotalColumns",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "TotalRows",
                table: "Rooms");
        }
    }
}
