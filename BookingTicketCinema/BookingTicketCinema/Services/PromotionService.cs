using BookingTicketCinema.DTO.Promotion;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepo;

        public PromotionService(IPromotionRepository promotionRepo)
        {
            _promotionRepo = promotionRepo;
        }

        public async Task<Promotion> CreatePromotionAsync(PromotionCreateDto dto)
        {
            if (dto.EndDate < dto.StartDate)
                throw new Exception("Ngày kết thúc không thể trước ngày bắt đầu.");

            if (await _promotionRepo.CodeExistsAsync(dto.Code))
                throw new Exception($"Mã khuyến mãi '{dto.Code}' đã tồn tại.");

            var promotion = new Promotion
            {
                Code = dto.Code,
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Description = dto.Description,
                IsActive = dto.IsActive,
            };

            return await _promotionRepo.AddAsync(promotion);
        }
        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync()
        {
            var promotions = await _promotionRepo.GetAllAsync();

            // Map Model sang DTO (bao gồm cả đếm số lần dùng)
            return promotions.Select(p => new PromotionDto
            {
                PromotionId = p.PromotionId,
                Code = p.Code,
                DiscountPercent = p.DiscountPercent,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                Description = p.Description,
                UsageCount = p.VoucherRedemptions.Count // Đếm số lần sử dụng
            });
        }

        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            return await _promotionRepo.GetByIdAsync(id);
        }

        public async Task<Promotion> UpdatePromotionAsync(int id, PromotionUpdateDto dto)
        {
            var existingPromotion = await _promotionRepo.GetByIdAsync(id);
            if (existingPromotion == null)
                throw new Exception("Không tìm thấy mã khuyến mãi.");

            if (dto.EndDate < dto.StartDate)
                throw new Exception("Ngày kết thúc không thể trước ngày bắt đầu.");

            // Map DTO -> Model (Chỉ cập nhật các trường được phép)
            existingPromotion.DiscountPercent = dto.DiscountPercent;
            existingPromotion.StartDate = dto.StartDate;
            existingPromotion.EndDate = dto.EndDate;
            existingPromotion.Description = dto.Description;
            existingPromotion.IsActive = dto.IsActive;
            existingPromotion.UpdatedAt = DateTime.UtcNow;

            return await _promotionRepo.UpdateAsync(existingPromotion);
        }
    }
}
