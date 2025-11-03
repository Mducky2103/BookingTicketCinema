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
    public class PromotionController : ControllerBase
    {
        private readonly CinemaDbContext _context;

        public PromotionController(CinemaDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả khuyến mãi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _context.Promotions
                .Include(p => p.VoucherRedemptions)
                .ThenInclude(v => v.User)
                .ToListAsync();
            return Ok(promotions);
        }

        // Lấy chi tiết khuyến mãi
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promo = await _context.Promotions
                .Include(p => p.VoucherRedemptions)
                .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(p => p.PromotionId == id);

            if (promo == null)
                return NotFound("Không tìm thấy khuyến mãi.");

            return Ok(promo);
        }

        // Tạo mới khuyến mãi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Promotion promotion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            promotion.Code = promotion.Code.ToUpper();
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = promotion.PromotionId }, promotion);
        }

        // Cập nhật khuyến mãi
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Promotion promotion)
        {
            if (id != promotion.PromotionId)
                return BadRequest("ID không khớp.");

            _context.Entry(promotion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Xóa khuyến mãi
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null)
                return NotFound();

            _context.Promotions.Remove(promo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Áp dụng mã giảm giá (dùng code)
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromQuery] string code, [FromQuery] decimal totalPrice)
        {
            var promo = await _context.Promotions.FirstOrDefaultAsync(p =>
                p.Code == code && p.IsActive &&
                p.StartDate <= DateOnly.FromDateTime(DateTime.Now) &&
                p.EndDate >= DateOnly.FromDateTime(DateTime.Now));

            if (promo == null)
                return BadRequest("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

            var discounted = totalPrice * (1 - promo.DiscountPercent);
            return Ok(new { totalPrice, discounted, promo.Code, promo.DiscountPercent });
        }
    }
}
