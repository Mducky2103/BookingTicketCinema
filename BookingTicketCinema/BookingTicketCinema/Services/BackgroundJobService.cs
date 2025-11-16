using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;
using static BookingTicketCinema.Models.Payment;
using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<BackgroundJobService> _logger;

        private const int EXPIRATION_MINUTES = 5;

        public BackgroundJobService(IPaymentRepository paymentRepository, ILogger<BackgroundJobService> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task CancelExpiredPaymentsAsync()
        {
            _logger.LogInformation("--- [Hangfire Job] Đang chạy tác vụ dọn dẹp vé hết hạn...");

            var expiredPayments = await _paymentRepository.FindExpiredPendingPaymentsAsync(EXPIRATION_MINUTES);

            if (!expiredPayments.Any())
            {
                _logger.LogInformation("--- [Hangfire Job] Không tìm thấy vé nào hết hạn.");
                return;
            }

            foreach (var payment in expiredPayments)
            {
                payment.Status = PaymentStatus.Cancelled; 

                foreach (var ticket in payment.Tickets)
                {
                    ticket.Status = TicketStatus.Cancelled; 
                }

                await _paymentRepository.UpdateAsync(payment);
            }

            _logger.LogInformation($"--- [Hangfire Job] Đã dọn dẹp {expiredPayments.Count} đơn hàng.");
        }
    }
}
