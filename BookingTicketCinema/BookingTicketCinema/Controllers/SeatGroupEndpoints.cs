using BookingTicketCinema.DTO;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    public static class SeatGroupEndpoints
    {
        public static IEndpointRouteBuilder MapSeatGroupEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/seatgroups", GetAllSeatGroups);
            app.MapGet("/seatgroups/{id}", GetSeatGroupById);
            app.MapGet("/seatgroups/room/{roomId}", GetSeatGroupsByRoom);
            app.MapPost("/seatgroups", CreateSeatGroup);
            app.MapPut("/seatgroups/{id}", UpdateSeatGroup);
            app.MapDelete("/seatgroups/{id}", DeleteSeatGroup);
            return app;
        }

        private static async Task<IResult> GetAllSeatGroups(ISeatGroupService seatGroupService)
        {
            var seatGroups = await seatGroupService.GetAllAsync();
            return Results.Ok(seatGroups);
        }

        private static async Task<IResult> GetSeatGroupById(int id, ISeatGroupService seatGroupService)
        {
            var seatGroup = await seatGroupService.GetByIdAsync(id);
            if (seatGroup == null) return Results.NotFound(new { message = "SeatGroup not found" });
            return Results.Ok(seatGroup);
        }

        private static async Task<IResult> GetSeatGroupsByRoom(int roomId, ISeatGroupService seatGroupService)
        {
            var seatGroups = await seatGroupService.GetByRoomIdAsync(roomId);
            return Results.Ok(seatGroups);
        }

        private static async Task<IResult> CreateSeatGroup([FromBody] CreateSeatGroupDto dto, ISeatGroupService seatGroupService)
        {
            try
            {
                var seatGroup = await seatGroupService.CreateAsync(dto);
                return Results.Created($"/seatgroups/{seatGroup.SeatGroupId}", seatGroup);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> UpdateSeatGroup(int id, [FromBody] UpdateSeatGroupDto dto, ISeatGroupService seatGroupService)
        {
            try
            {
                var seatGroup = await seatGroupService.UpdateAsync(id, dto);
                if (seatGroup == null) return Results.NotFound(new { message = "SeatGroup not found" });
                return Results.Ok(seatGroup);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }

        private static async Task<IResult> DeleteSeatGroup(int id, ISeatGroupService seatGroupService)
        {
            var result = await seatGroupService.DeleteAsync(id);
            if (!result) return Results.NotFound(new { message = "SeatGroup not found" });
            return Results.Ok(new { message = "SeatGroup deleted successfully" });
        }
    }
}


