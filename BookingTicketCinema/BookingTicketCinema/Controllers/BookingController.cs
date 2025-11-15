//using BookingTicketCinema.Data;
//using BookingTicketCinema.DTO;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Migrations.Operations;
//using Org.BouncyCastle.Bcpg;

//namespace BookingTicketCinema.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
    
//    public class BookingController : ControllerBase
//    {
//        private readonly CinemaDbContext _context;

//        public BookingController(CinemaDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("book")]
//        public async Task<IActionResult> BookTickets([FromBody] BookingRequestDTO request)
//        {
//            var showtime = await _context.Showtimes.Include(s => s.Room).FirstOrDefaultAsync(s => s.ShowtimeId == request.ShowtimeId);
//            if (showtime == null)
//            {
//                return NotFound("Showtime này không tồn tại.");
//            }

//            var reservedSeats = await _context.Tickets
//                .Where(t => request.SeatIds.Contains(t.SeatId) && t.ShowtimeId == request.ShowtimeId && t.Status != Models.Ticket.TicketStatus.Cancelled).ToListAsync();

//            if (reservedSeats.Any())
//            {
//                return BadRequest("Một số ghế đã được đặt trước.");
//            }


//            var tickets = request.SeatIds.Select(seatId => new Models.Ticket
//            {
//                ShowtimeId = request.ShowtimeId,
//                SeatId = seatId,
//                UserId = request.UserId,
//                Status = Models.Ticket.TicketStatus.Reserved,
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow
//            }).ToList();

//            _context.Tickets.AddRange(tickets);
//            await _context.SaveChangesAsync();

//            var ticketResponses = tickets.Select(t => new TicketResponseDTO
//            {
//                TicketId = t.TicketId,
//                SeatId = t.SeatId,
//                ShowtimeId = t.ShowtimeId,
//                UserId = t.UserId,
//                Status = t.Status.ToString(),
//                CreatedAt = t.CreatedAt
//            }).ToList();

//            return Ok(new
//            {
//                message = "Đặt vé thành công.",
//                count = ticketResponses.Count,
//                tickets = ticketResponses
//            });
//        }
//    }
//}
    
//public class BookingRequestDTO
//{
//    public int ShowtimeId { get; set; }
//    public List<int> SeatIds { get; set; } = new();
//    public string UserId { get; set; } = string.Empty;
//}