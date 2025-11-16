using BookingTicketCinema.DTO.Promotion;
using BookingTicketCinema.Models;

namespace BookingTicketCinema.Services.Interface
{
    public interface IPromotionService
    {
        Task<Promotion> CreatePromotionAsync(PromotionCreateDto dto);
        Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync();
        Task<Promotion?> GetPromotionByIdAsync(int id); 
        Task<Promotion> UpdatePromotionAsync(int id, PromotionUpdateDto dto);
    }
}
