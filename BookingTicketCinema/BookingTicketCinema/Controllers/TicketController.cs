using System.Security.Claims;
using BookingTicketCinema.DTO.Booking;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bảo vệ toàn bộ Controller
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // API MỚI (THAY THẾ BookingController)
        // POST: api/ticket/book
        [HttpPost("book")]
        [Authorize(Roles = "Customer")] 
        public async Task<IActionResult> BookTickets([FromBody] BookingRequestDTO request)
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var ticketResponses = await _ticketService.BookTicketsAsync(request, userId);
                return Ok(new
                {
                    message = "Đặt vé thành công.",
                    count = ticketResponses.Count,
                    tickets = ticketResponses
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // API MỚI (BẮT BUỘC CHO CLIENT CHỌN GHẾ)
        // GET: api/ticket/showtime/123/taken-seats
        [HttpGet("showtime/{showtimeId}/taken-seats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTakenSeats(int showtimeId)
        {
            var seatIds = await _ticketService.GetTakenSeatIdsAsync(showtimeId);
            return Ok(seatIds);
        }

        // GET: api/ticket/my-history
        [HttpGet("my-history")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyTicketHistory()
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tickets = await _ticketService.GetTicketHistoryAsync(userId);
            return Ok(tickets);
        }

        // PUT: api/ticket/55/status
        [HttpPut("{ticketId}/status")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> UpdateTicketStatus(int ticketId, [FromBody] UpdateTicketStatusDTO dto)
        {
            var ticket = await _ticketService.UpdateTicketStatusAsync(ticketId, dto.Status);
            if (ticket == null)
            {
                return NotFound("Vé không tồn tại.");
            }
            return Ok(new { message = "Cập nhật trạng thái thành công.", ticket });
        }
    }
}
