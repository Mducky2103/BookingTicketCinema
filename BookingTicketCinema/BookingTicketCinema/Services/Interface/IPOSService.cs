using BookingTicketCinema.DTO.Payment;
using BookingTicketCinema.DTO.POS;

namespace BookingTicketCinema.Services.Interface
{
    public interface IPOSService
    {
        Task<PaymentResponseDto> CreateBookingAtCounterAsync(POSRequestDto dto, string staffUserId);
        Task<PaymentResponseDto> GetReceiptAsync(int paymentId);
        Task<PagedResultDto<PaymentHistoryDto>> GetMyBookingHistoryAsync(string staffUserId, int pageNumber, int pageSize);
    }
}
