using BookingTicketCinema.DTO;

namespace BookingTicketCinema.Services.Interface
{
    public interface IPriceRuleService
    {
        Task<IEnumerable<PriceRuleResponseDto>> GetAllAsync();
        Task<PriceRuleResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PriceRuleResponseDto>> GetBySeatGroupIdAsync(int seatGroupId);
        Task<PriceRuleResponseDto> CreateAsync(CreatePriceRuleDto dto);
        Task<PriceRuleResponseDto?> UpdateAsync(int id, UpdatePriceRuleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}


