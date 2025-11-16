using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IPromotionRepository
    {
        Task<Promotion> AddAsync(Promotion promotion);
        Task<bool> CodeExistsAsync(string code);
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(int id);
        Task<Promotion> UpdateAsync(Promotion promotion);
        Task<Promotion?> GetByCodeAsync(string code);
    }
}
