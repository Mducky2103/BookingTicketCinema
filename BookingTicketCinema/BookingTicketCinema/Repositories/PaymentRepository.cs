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
            return await _context.Payments
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Showtime)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            // (Không cần gọi _context.Tickets.UpdateRange, 
            // EF Core tự động cập nhật Tickets khi Payment được cập nhật)
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Payment>> FindExpiredPendingPaymentsAsync(int minutes)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);

            return await _context.Payments
                .Include(p => p.Tickets) 
                .Where(p => p.Status == Payment.PaymentStatus.Pending && p.CreatedAt < cutoffTime)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .CountAsync();
        }

        public async Task<List<Payment>> GetByUserIdPagedAsync(string userId, int pageNumber, int pageSize)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Seat)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                .ToListAsync();
        }
    }
}
