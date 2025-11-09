using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    public static class ShowtimeEndpoints
    {
        public static IEndpointRouteBuilder MapShowtimeEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/showtimes/GetAllShowtime", GetAllShowtimes);
            app.MapPost("/api/showtimes/CreateShowtime", CreateShowtime);
            app.MapPut("/api/showtimes/UpdateShowtime/{id}", UpdateShowtime);
            app.MapDelete("/api/showtimes/DeleteShowtime/{id}", DeleteShowtime);
            app.MapGet("/api/showtimes/GetShowtimeById/{id}", GetShowtimeById);
            app.MapGet("/api/showtimes/GetShowtimesByRoom/{roomId}", GetShowtimesByRoom);
            return app;
        }
        [Authorize(Roles = "Admin, Staff")]
        private static async Task<IResult> CreateShowtime(
            [FromBody] ShowTimeCreateDto showtimeCreateDto,
            IShowtimeService showtimeService)
        {
            var createdShowtime = await showtimeService.CreateAsync(showtimeCreateDto);
            return Results.Ok(createdShowtime);
        }
        [Authorize(Roles = "Admin, Staff")]
        private static async Task<IResult> UpdateShowtime(
            int id,
            [FromBody] ShowTimeUpdateDto showtimeUpdateDto,
            IShowtimeService showtimeService)
        {
            var updatedShowtime = await showtimeService.UpdateAsync(id, showtimeUpdateDto);
            if (updatedShowtime == null) return Results.NotFound(new { message = "Showtime not found" });
            return Results.Ok(updatedShowtime);
        }
        [Authorize(Roles = "Admin, Staff")]
        private static async Task<IResult> DeleteShowtime(
            int id,
            IShowtimeService showtimeService)
        {
            var isDeleted = await showtimeService.DeleteAsync(id);
            if (!isDeleted) return Results.NotFound(new { message = "Showtime not found" });
            return Results.Ok(new { message = "Showtime deleted successfully" });
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


