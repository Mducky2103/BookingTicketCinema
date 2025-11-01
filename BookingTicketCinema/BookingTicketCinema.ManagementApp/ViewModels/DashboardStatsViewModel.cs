namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class DashboardStatsViewModel
    {
        public decimal TodayRevenue { get; set; }
        public int TicketsSoldToday { get; set; }
        public double OccupancyRate { get; set; }
    }
}
