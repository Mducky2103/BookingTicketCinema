using System.Security.Cryptography;
using System.Text;
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
    public class VoucherController : ControllerBase
    {
        private readonly CinemaDbContext _context;
        public VoucherController(CinemaDbContext context) => _context = context;

        // Lấy danh sách khuyến mãi (voucher)
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Promotions.ToListAsync());

        // Sinh mã giảm giá ngẫu nhiên
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateVoucher([FromBody] Promotion promo)
        {
            // Sinh mã code ngẫu nhiên 8 ký tự (chữ + số)
            promo.Code = GenerateRandomCode(8);

            // Chuẩn hóa các giá trị khác
            promo.Code = promo.Code.ToUpper();
            promo.CreatedAt = DateTime.UtcNow;
            promo.UpdatedAt = DateTime.UtcNow;
            promo.IsActive = true;

            _context.Promotions.Add(promo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                promo.PromotionId,
                promo.Code,
                promo.DiscountPercent,
                promo.StartDate,
                promo.EndDate,
                promo.Description
            });
        }

        // 🔹 Hàm sinh mã giảm giá ngẫu nhiên
        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var sb = new StringBuilder(length);
            foreach (var b in data)
                sb.Append(chars[b % chars.Length]);

            return sb.ToString();
        }

        // Áp dụng mã giảm giá (check hợp lệ)
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyVoucher(string code, decimal totalPrice)
        {
            var promo = await _context.Promotions.FirstOrDefaultAsync(p =>
                p.Code == code && p.IsActive && p.StartDate <= DateOnly.FromDateTime(DateTime.Now)
                && p.EndDate >= DateOnly.FromDateTime(DateTime.Now));

            if (promo == null)
                return BadRequest("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

            var finalPrice = totalPrice * (1 - promo.DiscountPercent);
            return Ok(new { Original = totalPrice, Final = finalPrice, Discount = promo.DiscountPercent });
        }
    }
}
