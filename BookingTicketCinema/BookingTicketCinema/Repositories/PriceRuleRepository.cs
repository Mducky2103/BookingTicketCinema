using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class PriceRuleRepository : IPriceRuleRepository
    {
        private readonly CinemaDbContext _context;

        public PriceRuleRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PriceRule>> GetAllAsync() =>
            await _context.PriceRules.ToListAsync();

        public async Task<PriceRule?> GetByIdAsync(int id) =>
            await _context.PriceRules.FindAsync(id);

        public async Task<IEnumerable<PriceRule>> GetBySeatGroupIdAsync(int seatGroupId) =>
            await _context.PriceRules.Where(pr => pr.SeatGroupId == seatGroupId).ToListAsync();

        public async Task AddAsync(PriceRule priceRule) =>
            await _context.PriceRules.AddAsync(priceRule);

        public async Task UpdateAsync(PriceRule priceRule) =>
            _context.PriceRules.Update(priceRule);

        public async Task DeleteAsync(PriceRule priceRule) =>
            _context.PriceRules.Remove(priceRule);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}

