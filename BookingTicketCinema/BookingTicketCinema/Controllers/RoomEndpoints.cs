using BookingTicketCinema.DTO;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    public static class RoomEndpoints
    {
        public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/rooms", GetAllRooms);
            app.MapGet("/rooms/{id}", GetRoomById);
            app.MapPost("/rooms", CreateRoom);
            app.MapPut("/rooms/{id}", UpdateRoom);
            app.MapDelete("/rooms/{id}", DeleteRoom);
            return app;
        }

        private static async Task<IResult> GetAllRooms(IRoomService roomService)
        {
            var rooms = await roomService.GetAllAsync();
            return Results.Ok(rooms);
        }

        private static async Task<IResult> GetRoomById(int id, IRoomService roomService)
        {
            var room = await roomService.GetByIdAsync(id);
            if (room == null) return Results.NotFound(new { message = "Room not found" });
            return Results.Ok(room);
        }

        private static async Task<IResult> CreateRoom([FromBody] CreateRoomDto dto, IRoomService roomService)
        {
            try
            {
                var room = await roomService.CreateAsync(dto);
                return Results.Created($"/rooms/{room.RoomId}", room);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRoom(int id, [FromBody] UpdateRoomDto dto, IRoomService roomService)
        {
            try
            {
                var room = await roomService.UpdateAsync(id, dto);
                if (room == null) return Results.NotFound(new { message = "Room not found" });
                return Results.Ok(room);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRoom(int id, IRoomService roomService)
        {
            var result = await roomService.DeleteAsync(id);
            if (!result) return Results.NotFound(new { message = "Room not found" });
            return Results.Ok(new { message = "Room deleted successfully" });
        }
    }
}

