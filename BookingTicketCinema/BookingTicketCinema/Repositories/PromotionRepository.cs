using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly CinemaDbContext _context;

        public PromotionRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<Promotion> AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Promotions
                .AnyAsync(p => p.Code.ToLower() == code.ToLower());
        }
        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            // Include(p => p.VoucherRedemptions) để đếm số lần sử dụng
            return await _context.Promotions
                .Include(p => p.VoucherRedemptions)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Promotion?> GetByIdAsync(int id)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromotionId == id);
        }

        public async Task<Promotion> UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }
        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code.ToLower() == code.ToLower());
        }
    }
}
