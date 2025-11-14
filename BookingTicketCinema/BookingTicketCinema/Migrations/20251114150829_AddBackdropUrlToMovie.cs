using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicketCinema.Migrations
{
    /// <inheritdoc />
    public partial class AddBackdropUrlToMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackdropUrl",
                table: "Movies",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackdropUrl",
                table: "Movies");
        }
    }
}
