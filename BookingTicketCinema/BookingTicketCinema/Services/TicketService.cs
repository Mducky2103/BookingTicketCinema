using BookingTicketCinema.DTO.Booking;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;
using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        // (Bạn nên inject thêm IShowtimeRepository để kiểm tra Showtime tồn tại)

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // 1. LOGIC ĐẶT VÉ (Từ BookingController)
        public async Task<List<TicketResponseDTO>> BookTicketsAsync(BookingRequestDTO dto, string userId)
        {
            // (Bạn nên thêm logic kiểm tra ShowtimeId có tồn tại không)
            // (Bạn nên thêm logic kiểm tra giá vé từ PriceRules)

            // Kiểm tra ghế (Logic gốc của bạn)
            var reservedSeats = await _ticketRepository.GetByShowtimeIdAsync(dto.ShowtimeId);
            var reservedSeatIds = reservedSeats.Select(t => t.SeatId).ToHashSet();

            if (dto.SeatIds.Any(id => reservedSeatIds.Contains(id)))
            {
                throw new Exception("Một số ghế đã được đặt trước.");
            }

            var tickets = dto.SeatIds.Select(seatId => new Ticket
            {
                ShowtimeId = dto.ShowtimeId,
                SeatId = seatId,
                UserId = userId, // <-- SỬA LỖI BẢO MẬT: Lấy UserId từ Token
                Status = TicketStatus.Reserved, // (Hoặc 'Booked' nếu thanh toán ngay)
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _ticketRepository.AddRangeAsync(tickets);

            // Map sang DTO để trả về
            return tickets.Select(t => new TicketResponseDTO
            {
                TicketId = t.TicketId,
                SeatId = t.SeatId,
                ShowtimeId = t.ShowtimeId,
                UserId = t.UserId,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        // 2. LOGIC LỊCH SỬ (Từ TicketManagement)
        public async Task<IEnumerable<TicketHistoryDto>> GetTicketHistoryAsync(string userId)
        {
            var tickets = await _ticketRepository.GetByUserIdAsync(userId);

            // Map sang DTO
            return tickets.Select(t => new TicketHistoryDto
            {
                TicketId = t.TicketId,
                Movie = t.Showtime.Movie.Title,
                PosterUrl = t.Showtime.Movie.PosterUrl,
                StartTime = t.Showtime.StartTime,
                RoomName = t.Showtime.Room.Name,
                Seat = t.Seat.SeatNumber,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            });
        }

        // 3. LOGIC CẬP NHẬT STATUS (Từ TicketManagement)
        public async Task<Ticket?> UpdateTicketStatusAsync(int ticketId, TicketStatus status)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return null;

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _ticketRepository.UpdateAsync(ticket);
            return ticket;
        }

        // 4. API CÒN THIẾU (Lấy ghế đã bán)
        public async Task<IEnumerable<int>> GetTakenSeatIdsAsync(int showtimeId)
        {
            var tickets = await _ticketRepository.GetByShowtimeIdAsync(showtimeId);
            return tickets.Select(t => t.SeatId).ToList();
        }
    }
}
