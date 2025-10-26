using BookingTicketCinema.DTO;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    public static class SeatEndpoints
    {
        public static IEndpointRouteBuilder MapSeatEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/seats", GetAllSeats);
            app.MapGet("/seats/{id}", GetSeatById);
            app.MapGet("/seats/room/{roomId}", GetSeatsByRoom);
            app.MapPost("/seats", CreateSeat);
            app.MapPut("/seats/{id}", UpdateSeat);
            app.MapDelete("/seats/{id}", DeleteSeat);
            return app;
        }

        private static async Task<IResult> GetAllSeats(ISeatService seatService)
        {
            var seats = await seatService.GetAllAsync();
            return Results.Ok(seats);
        }

        private static async Task<IResult> GetSeatById(int id, ISeatService seatService)
        {
            var seat = await seatService.GetByIdAsync(id);
            if (seat == null) return Results.NotFound(new { message = "Seat not found" });
            return Results.Ok(seat);
        }

        private static async Task<IResult> GetSeatsByRoom(int roomId, ISeatService seatService)
        {
            var seats = await seatService.GetByRoomIdAsync(roomId);
            return Results.Ok(seats);
        }

        private static async Task<IResult> CreateSeat([FromBody] CreateSeatDto dto, ISeatService seatService)
        {
            try
            {
                var seat = await seatService.CreateAsync(dto);
                return Results.Created($"/seats/{seat.SeatId}", seat);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateSeat(int id, [FromBody] UpdateSeatDto dto, ISeatService seatService)
        {
            try
            {
                var seat = await seatService.UpdateAsync(id, dto);
                if (seat == null) return Results.NotFound(new { message = "Seat not found" });
                return Results.Ok(seat);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> DeleteSeat(int id, ISeatService seatService)
        {
            var result = await seatService.DeleteAsync(id);
            if (!result) return Results.NotFound(new { message = "Seat not found" });
            return Results.Ok(new { message = "Seat deleted successfully" });
        }
    }
}


