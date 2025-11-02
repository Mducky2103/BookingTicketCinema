using BookingTicketCinema.DTO;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    public static class ShowtimeEndpoints
    {
        public static IEndpointRouteBuilder MapShowtimeEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/showtimes", GetAllShowtimes);
            app.MapGet("/showtimes/{id}", GetShowtimeById);
            app.MapGet("/showtimes/room/{roomId}", GetShowtimesByRoom);
            return app;
        }

        [AllowAnonymous]
        private static async Task<IResult> GetAllShowtimes(IShowtimeService showtimeService)
        {
            var showtimes = await showtimeService.GetAllAsync();
            return Results.Ok(showtimes);
        }

        [AllowAnonymous]
        private static async Task<IResult> GetShowtimeById(int id, IShowtimeService showtimeService)
        {
            var showtime = await showtimeService.GetByIdAsync(id);
            if (showtime == null) return Results.NotFound(new { message = "Showtime not found" });
            return Results.Ok(showtime);
        }

        [AllowAnonymous]
        private static async Task<IResult> GetShowtimesByRoom(int roomId, IShowtimeService showtimeService)
        {
            var showtimes = await showtimeService.GetByRoomIdAsync(roomId);
            return Results.Ok(showtimes);
        }
    }
}


