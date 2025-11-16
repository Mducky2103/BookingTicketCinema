namespace BookingTicketCinema.Services.Interface
{
    public interface IBackgroundJobService
    {
        Task CancelExpiredPaymentsAsync();
    }
}
