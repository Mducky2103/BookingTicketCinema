using System.Security.Claims;
using BookingTicketCinema.DTO.POS;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Staff")] 
    public class POSController : ControllerBase
    {
        private readonly IPOSService _posService;

        public POSController(IPOSService posService)
        {
            _posService = posService;
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookAtCounter([FromBody] POSRequestDto request)
        {
            var staffUserId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(staffUserId))
            {
                return Unauthorized();
            }

            if (request.SeatIds == null || !request.SeatIds.Any())
            {
                return BadRequest(new { message = "Vui lòng chọn ít nhất 1 ghế." });
            }

            try
            {
                var paymentResponse = await _posService.CreateBookingAtCounterAsync(request, staffUserId);
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("receipt/{paymentId}")]
        public async Task<IActionResult> GetReceipt(int paymentId)
        {
            try
            {
                var receipt = await _posService.GetReceiptAsync(paymentId);
                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyPOSHistory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            var staffUserId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(staffUserId)) return Unauthorized();

            try
            {
                var result = await _posService.GetMyBookingHistoryAsync(staffUserId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
