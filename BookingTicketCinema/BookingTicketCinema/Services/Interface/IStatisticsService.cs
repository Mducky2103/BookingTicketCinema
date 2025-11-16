using BookingTicketCinema.DTO.Statistic;

namespace BookingTicketCinema.Services.Interface
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetDashboardStatisticsAsync();
        Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}
