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
            app.MapGet("/showtimes/GetAllShowtime", GetAllShowtimes);
            app.MapPost("/showtimes/CreateShowtime", CreateShowtime);
            app.MapPut("/showtimes/UpdateShowtime/{id}", UpdateShowtime);
            app.MapDelete("/showtimes/DeleteShowtime/{id}", DeleteShowtime);
            app.MapGet("/showtimes/GetShowtimeById/{id}", GetShowtimeById);
            app.MapGet("/showtimes/GetShowtimesByRoom/{roomId}", GetShowtimesByRoom);
            app.MapGet("/showtimes/GetShowtimesByMovie/{movieId}", GetShowtimesByMovieId);
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
        [FromBody] ShowTimeUpdateDto dto,
            IShowtimeService showtimeService)
        {
            if (dto == null)
                return Results.BadRequest(new { message = "Invalid request body." });

            var result = await showtimeService.UpdateAsync(id, dto);

            if (result == null)
                return Results.NotFound(new { message = $"Showtime with ID {id} not found." });

            return Results.Ok(new
            {
                message = "Showtime updated successfully.",
                data = result
            });
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

        [AllowAnonymous]
        private static async Task<IResult> GetShowtimesByMovieId(int movieId, IShowtimeService showtimeService)
        {
            var showtimes = await showtimeService.GetByMovieIdAsync(movieId);
            return Results.Ok(showtimes);
        }
    }
}


