using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicketCinema.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PromotionId",
                table: "Payments",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Promotions_PromotionId",
                table: "Payments",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Promotions_PromotionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PromotionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Payments");
        }
    }
}
