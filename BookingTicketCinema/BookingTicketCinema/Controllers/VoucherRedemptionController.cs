using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherRedemptionController : ControllerBase
    {
        private readonly CinemaDbContext _context;

        public VoucherRedemptionController(CinemaDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ lịch sử sử dụng mã
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.VoucherRedemptions
                .Include(v => v.Promotion)
                .Include(v => v.User)
                .ToListAsync();
            return Ok(list);
        }

        // Ghi nhận việc sử dụng mã
        [HttpPost]
        public async Task<IActionResult> Redeem([FromQuery] int promotionId, [FromQuery] string userId)
        {
            var promo = await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == promotionId && p.IsActive);
            if (promo == null)
                return BadRequest("Không tìm thấy chương trình khuyến mãi hợp lệ.");

            var redemption = new VoucherRedemption
            {
                PromotionId = promo.PromotionId,
                UserId = userId,
                UsedAt = DateTime.UtcNow
            };

            _context.VoucherRedemptions.Add(redemption);
            await _context.SaveChangesAsync();

            return Ok("Đã ghi nhận việc sử dụng mã khuyến mãi.");
        }
    }
}
