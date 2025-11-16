using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IVoucherRedemptionRepository
    {
        Task AddAsync(VoucherRedemption redemption);
        Task<bool> HasUserRedeemedAsync(int promotionId, string userId);
    }
}
