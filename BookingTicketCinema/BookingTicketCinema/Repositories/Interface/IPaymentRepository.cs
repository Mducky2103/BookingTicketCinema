using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByIdAsync(int paymentId);
        Task UpdateAsync(Payment payment);
    }
}
