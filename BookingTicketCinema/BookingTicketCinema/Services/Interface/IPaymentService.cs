using BookingTicketCinema.DTO.Payment;

namespace BookingTicketCinema.Services.Interface
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto dto, string userId);
        Task<bool> ConfirmPaymentAsync(int paymentId, string userId);
        Task<bool> CancelPaymentAsync(int paymentId, string userId);
    }
}
