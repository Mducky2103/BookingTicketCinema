using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IPriceRuleRepository
    {
        Task<IEnumerable<PriceRule>> GetAllAsync();
        Task<PriceRule?> GetByIdAsync(int id);
        Task<IEnumerable<PriceRule>> GetBySeatGroupIdAsync(int seatGroupId);
        Task AddAsync(PriceRule priceRule);
        Task UpdateAsync(PriceRule priceRule);
        Task DeleteAsync(PriceRule priceRule);
        Task SaveChangesAsync();
    }
}


