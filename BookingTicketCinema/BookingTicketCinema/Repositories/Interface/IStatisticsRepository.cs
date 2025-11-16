namespace BookingTicketCinema.Repositories.Interface
{
    public interface IStatisticsRepository
    {
        Task<decimal> GetTodayRevenueAsync();
        Task<int> GetTodayTicketsSoldAsync();
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalMoviesAsync();
        Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTicketsSoldByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
