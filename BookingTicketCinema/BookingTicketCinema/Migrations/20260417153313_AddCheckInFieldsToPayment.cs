using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicketCinema.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInFieldsToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingCode",
                table: "Payments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "Payments");
        }
    }
}
