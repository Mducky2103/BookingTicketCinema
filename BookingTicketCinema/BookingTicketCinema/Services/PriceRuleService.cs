using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class PriceRuleService : IPriceRuleService
    {
        private readonly IPriceRuleRepository _priceRuleRepository;
        private readonly ISeatGroupRepository _seatGroupRepository;

        public PriceRuleService(IPriceRuleRepository priceRuleRepository, ISeatGroupRepository seatGroupRepository)
        {
            _priceRuleRepository = priceRuleRepository;
            _seatGroupRepository = seatGroupRepository;
        }

        public async Task<IEnumerable<PriceRuleResponseDto>> GetAllAsync()
        {
            var priceRules = await _priceRuleRepository.GetAllAsync();
            return priceRules.Select(pr => new PriceRuleResponseDto
            {
                PriceRuleId = pr.PriceRuleId,
                BasePrice = pr.BasePrice,
                DayOfWeek = pr.DayOfWeek,
                Slot = pr.Slot,
                SeatGroupId = pr.SeatGroupId,
                ShowtimeId = pr.ShowtimeId,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt
            });
        }

        public async Task<PriceRuleResponseDto?> GetByIdAsync(int id)
        {
            var priceRule = await _priceRuleRepository.GetByIdAsync(id);
            if (priceRule == null) return null;

            return new PriceRuleResponseDto
            {
                PriceRuleId = priceRule.PriceRuleId,
                BasePrice = priceRule.BasePrice,
                DayOfWeek = priceRule.DayOfWeek,
                Slot = priceRule.Slot,
                SeatGroupId = priceRule.SeatGroupId,
                ShowtimeId = priceRule.ShowtimeId,
                CreatedAt = priceRule.CreatedAt,
                UpdatedAt = priceRule.UpdatedAt
            };
        }

        public async Task<IEnumerable<PriceRuleResponseDto>> GetBySeatGroupIdAsync(int seatGroupId)
        {
            var priceRules = await _priceRuleRepository.GetBySeatGroupIdAsync(seatGroupId);
            return priceRules.Select(pr => new PriceRuleResponseDto
            {
                PriceRuleId = pr.PriceRuleId,
                BasePrice = pr.BasePrice,
                DayOfWeek = pr.DayOfWeek,
                Slot = pr.Slot,
                SeatGroupId = pr.SeatGroupId,
                ShowtimeId = pr.ShowtimeId,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt
            });
        }

        public async Task<PriceRuleResponseDto> CreateAsync(CreatePriceRuleDto dto)
        {
            var seatGroup = await _seatGroupRepository.GetByIdAsync(dto.SeatGroupId);
            if (seatGroup == null) throw new Exception("SeatGroup not found");

            var priceRule = new PriceRule
            {
                BasePrice = dto.BasePrice,
                DayOfWeek = dto.DayOfWeek,
                Slot = dto.Slot,
                SeatGroupId = dto.SeatGroupId,
                ShowtimeId = dto.ShowtimeId
            };

            await _priceRuleRepository.AddAsync(priceRule);
            await _priceRuleRepository.SaveChangesAsync();

            return new PriceRuleResponseDto
            {
                PriceRuleId = priceRule.PriceRuleId,
                BasePrice = priceRule.BasePrice,
                DayOfWeek = priceRule.DayOfWeek,
                Slot = priceRule.Slot,
                SeatGroupId = priceRule.SeatGroupId,
                ShowtimeId = priceRule.ShowtimeId,
                CreatedAt = priceRule.CreatedAt,
                UpdatedAt = priceRule.UpdatedAt
            };
        }

        public async Task<PriceRuleResponseDto?> UpdateAsync(int id, UpdatePriceRuleDto dto)
        {
            var priceRule = await _priceRuleRepository.GetByIdAsync(id);
            if (priceRule == null) return null;

            if (dto.BasePrice.HasValue) priceRule.BasePrice = dto.BasePrice.Value;
            if (dto.DayOfWeek.HasValue) priceRule.DayOfWeek = dto.DayOfWeek.Value;
            if (dto.Slot.HasValue) priceRule.Slot = dto.Slot.Value;
            if (dto.SeatGroupId.HasValue)
            {
                var seatGroup = await _seatGroupRepository.GetByIdAsync(dto.SeatGroupId.Value);
                if (seatGroup == null) throw new Exception("SeatGroup not found");
                priceRule.SeatGroupId = dto.SeatGroupId.Value;
            }
            if (dto.ShowtimeId.HasValue && dto.ShowtimeId.Value == -1)
            {
                priceRule.ShowtimeId = null;
            }
            else if (dto.ShowtimeId.HasValue)
            {
                priceRule.ShowtimeId = dto.ShowtimeId.Value;
            }

            await _priceRuleRepository.UpdateAsync(priceRule);
            await _priceRuleRepository.SaveChangesAsync();

            return new PriceRuleResponseDto
            {
                PriceRuleId = priceRule.PriceRuleId,
                BasePrice = priceRule.BasePrice,
                DayOfWeek = priceRule.DayOfWeek,
                Slot = priceRule.Slot,
                SeatGroupId = priceRule.SeatGroupId,
                ShowtimeId = priceRule.ShowtimeId,
                CreatedAt = priceRule.CreatedAt,
                UpdatedAt = priceRule.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var priceRule = await _priceRuleRepository.GetByIdAsync(id);
            if (priceRule == null) return false;

            await _priceRuleRepository.DeleteAsync(priceRule);
            await _priceRuleRepository.SaveChangesAsync();
            return true;
        }
    }
}


