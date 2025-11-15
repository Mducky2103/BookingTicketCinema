using BookingTicketCinema.DTO.Booking;
using BookingTicketCinema.Models;
using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.Services.Interface
{
    public interface ITicketService
    {
        // Hàm Đặt vé (Sửa lỗi bảo mật)
        Task<List<TicketResponseDTO>> BookTicketsAsync(BookingRequestDTO dto, string userId);

        // Hàm Lịch sử (Sửa lỗi bảo mật)
        Task<IEnumerable<TicketHistoryDto>> GetTicketHistoryAsync(string userId);

        // Hàm Cập nhật (cho Admin)
        Task<Ticket?> UpdateTicketStatusAsync(int ticketId, TicketStatus status);

        // API CÒN THIẾU (cho Client)
        Task<IEnumerable<int>> GetTakenSeatIdsAsync(int showtimeId);
    }
}
