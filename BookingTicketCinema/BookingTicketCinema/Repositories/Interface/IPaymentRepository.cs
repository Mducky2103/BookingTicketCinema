using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByIdAsync(int paymentId);
        Task UpdateAsync(Payment payment);
        Task<List<Payment>> FindExpiredPendingPaymentsAsync(int minutes);
        Task<List<Payment>> GetByUserIdPagedAsync(string userId, int pageNumber, int pageSize);
        Task<int> GetCountByUserIdAsync(string userId);
    }
}
