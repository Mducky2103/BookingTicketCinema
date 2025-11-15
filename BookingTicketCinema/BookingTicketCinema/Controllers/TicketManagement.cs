//using BookingTicketCinema.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using static BookingTicketCinema.Models.Ticket;

//namespace BookingTicketCinema.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TicketManagement : ControllerBase
//    {
//        private readonly CinemaDbContext _context;
//        public TicketManagement(CinemaDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPut("{ticketId}/status")]
//        public async Task<IActionResult> UpdateTicketStatus(int ticketId, [FromBody] UpdateTicketStatusDTO dto)
//        {
//            var ticket = await _context.Tickets.FindAsync(ticketId);
//            if (ticket == null)
//            {
//                return NotFound("Vé này không tồn tại trong hệ thống.");
//            }
//            ticket.Status = dto.Status;
//            ticket.UpdatedAt = DateTime.UtcNow;
//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                message = "Trạng thái của vé đã được cập nhật thành công.",
//                ticket
//            });
//        }
//        [HttpGet("user/{userId}")]
//        public async Task<IActionResult> GetTicketHistoryByUserId(string userId)
//        {
//            var tickets = await _context.Tickets.Include(t => t.Showtime).ThenInclude(s => s.Movie)
//                .Include(t => t.Seat)
//                .Where(t => t.UserId == userId)
//                .OrderByDescending(t => t.CreatedAt)
//                .Select(t => new
//                {
//                    t.TicketId,
//                    Movie = t.Showtime.Movie.Title,
//                    t.Showtime.StartTime,
//                    Seat = t.Seat.SeatNumber,
//                    t.Status,
//                    t.CreatedAt
//                }).ToListAsync();
//            return Ok(tickets);
//        }
//    }
//}
//public class UpdateTicketStatusDTO
//{
//    public TicketStatus Status { get; set; }
//}