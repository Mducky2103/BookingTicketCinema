using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly CinemaDbContext _context;
        private readonly UserManager<User> _userManager;

        public StatisticsRepository(CinemaDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            // Chỉ tính các đơn hàng đã "Hoàn thành"
            return await _context.Payments
                .Where(p => p.Status == Payment.PaymentStatus.Completed &&
                            p.CreatedAt.Date == DateTime.UtcNow.Date)
                .SumAsync(p => p.Amount);
        }

        public async Task<int> GetTodayTicketsSoldAsync()
        {
            // Chỉ tính các vé đã "Thanh toán" (Paid)
            return await _context.Tickets
                .Where(t => t.Status == Ticket.TicketStatus.Paid &&
                            t.CreatedAt.Date == DateTime.UtcNow.Date)
                .CountAsync();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            // Đếm tổng số User có Role "Customer"
            return (await _userManager.GetUsersInRoleAsync("Customer")).Count;
        }

        public async Task<int> GetTotalMoviesAsync()
        {
            // (Chỉ đếm phim đang hoạt động, nếu cần)
            return await _context.Movies.CountAsync();
        }
        public async Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // endDate.AddDays(1) để bao gồm cả ngày kết thúc (ví dụ: đến 23:59:59)
            var endDateTime = endDate.Date.AddDays(1);

            return await _context.Payments
                .Where(p => p.Status == Payment.PaymentStatus.Completed &&
                            p.CreatedAt >= startDate.Date &&
                            p.CreatedAt < endDateTime)
                .SumAsync(p => p.Amount);
        }

        public async Task<int> GetTicketsSoldByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var endDateTime = endDate.Date.AddDays(1);

            return await _context.Tickets
                .Where(t => t.Status == Ticket.TicketStatus.Paid &&
                            t.CreatedAt >= startDate.Date &&
                            t.CreatedAt < endDateTime)
                .CountAsync();
        }

        public async Task<int> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var endDateTime = endDate.Date.AddDays(1);

            return await _context.Payments
                .Where(p => p.Status == Payment.PaymentStatus.Completed &&
                            p.CreatedAt >= startDate.Date &&
                            p.CreatedAt < endDateTime)
                .CountAsync();
        }
    }
}
