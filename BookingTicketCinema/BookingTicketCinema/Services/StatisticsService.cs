
using BookingTicketCinema.DTO.Statistic;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statsRepository;

        public StatisticsService(IStatisticsRepository statsRepository)
        {
            _statsRepository = statsRepository;
        }

        public async Task<StatisticsDto> GetDashboardStatisticsAsync()
        {
            // (Chạy tuần tự để tránh lỗi "second operation" của DbContext)
            var todayRevenue = await _statsRepository.GetTodayRevenueAsync();
            var ticketsSoldToday = await _statsRepository.GetTodayTicketsSoldAsync();
            var totalCustomers = await _statsRepository.GetTotalCustomersAsync();
            var totalMovies = await _statsRepository.GetTotalMoviesAsync();

            return new StatisticsDto
            {
                TodayRevenue = todayRevenue,
                TicketsSoldToday = ticketsSoldToday,
                TotalCustomers = totalCustomers,
                TotalMovies = totalMovies
            };
        }
        public async Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            // Chạy tuần tự để tránh lỗi DbContext
            var revenue = await _statsRepository.GetRevenueByDateRangeAsync(startDate, endDate);
            var tickets = await _statsRepository.GetTicketsSoldByDateRangeAsync(startDate, endDate);
            var payments = await _statsRepository.GetPaymentsByDateRangeAsync(startDate, endDate);

            return new SalesReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = revenue,
                TotalTicketsSold = tickets,
                TotalPayments = payments
            };
        }
    }
}
