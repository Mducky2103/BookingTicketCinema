using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly CinemaDbContext _context;

        public PaymentRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            // Vì Payment chứa Tickets, EF Core sẽ tự động
            // thêm Payment và Tickets trong 1 giao dịch (transaction)
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            // Lấy Payment và các Ticket liên quan
            return await _context.Payments
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            // (Không cần gọi _context.Tickets.UpdateRange, 
            // EF Core tự động cập nhật Tickets khi Payment được cập nhật)
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
