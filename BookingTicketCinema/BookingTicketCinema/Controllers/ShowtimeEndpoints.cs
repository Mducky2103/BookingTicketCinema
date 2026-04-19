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

            // 1. Đăng ký Endpoint tạo hàng loạt (Bulk Create)
            app.MapPost("/showtimes/CreateBulkShowtime", CreateBulkShowtime);

            // 2. Đăng ký Endpoint kiểm tra trùng lịch nhanh (Check Overlap)
            app.MapGet("/showtimes/CheckOverlap", CheckOverlap);
            return app;
        }

        // ===================== PHẦN CODE TẠO HÀNG LOẠT =====================
        [Authorize(Roles = "Admin, Staff")]
        private static async Task<IResult> CreateBulkShowtime(
            [FromBody] ShowTimeBulkCreateDto bulkDto,
            IShowtimeService showtimeService)
        {
            if (bulkDto == null || bulkDto.StartTimes == null || !bulkDto.StartTimes.Any())
            {
                return Results.BadRequest(new { message = "Danh sách giờ bắt đầu không được để trống." });
            }

            // Gọi service xử lý logic bulk
            var result = await showtimeService.CreateBulkAsync(bulkDto);

            return Results.Ok(new
            {
                message = $"Đã xử lý xong. Thành công: {result.Success.Count}, Thất bại: {result.Errors.Count}",
                successData = result.Success,
                errors = result.Errors // Trả về danh sách các suất bị trùng để báo lỗi trên form
            });
        }

        // ===================== KIỂM TRA TRÙNG LỊCH NHANH =====================
        [AllowAnonymous]
        private static async Task<IResult> CheckOverlap(
            [FromQuery] int roomId,
            [FromQuery] DateTime startTime,
            [FromQuery] int movieId,
            IShowtimeService showtimeService)
        {
            // API này dùng để Frontend gọi ngay khi Admin vừa nhập giờ để báo lỗi đỏ tại chỗ
            var isOverlap = await showtimeService.IsOverlapAsync(roomId, startTime, movieId);

            return Results.Ok(new
            {
                isOverlap = isOverlap,
                message = isOverlap ? "Khung giờ này đã có suất chiếu khác!" : "Khung giờ hợp lệ."
            });
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
        private static async Task<IResult> GetAllShowtimes(
            IShowtimeService showtimeService,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 15)
        {
            // service now returns a PagedResult<ShowtimeResponseDto>
            var paged = await showtimeService.GetAllAsync(pageNumber, pageSize);
            return Results.Ok(paged);
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


