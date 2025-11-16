using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class VoucherRedemptionRepository : IVoucherRedemptionRepository
    {
        private readonly CinemaDbContext _context;

        public VoucherRedemptionRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VoucherRedemption redemption)
        {
            await _context.VoucherRedemptions.AddAsync(redemption);
            // (Lưu ý: SaveChangesAsync() sẽ được gọi ở Service)
        }

        public async Task<bool> HasUserRedeemedAsync(int promotionId, string userId)
        {
            return await _context.VoucherRedemptions
                .AnyAsync(r => r.PromotionId == promotionId && r.UserId == userId);
        }
    }
}
