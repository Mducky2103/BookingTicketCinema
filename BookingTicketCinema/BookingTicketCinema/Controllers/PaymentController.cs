using System.Security.Claims;
using BookingTicketCinema.DTO.Payment;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")] 
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (request.SeatIds == null || !request.SeatIds.Any())
            {
                return BadRequest(new { message = "Vui lòng chọn ít nhất 1 ghế." });
            }

            try
            {
                var paymentResponse = await _paymentService.CreatePaymentAsync(request, userId);
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{paymentId}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _paymentService.ConfirmPaymentAsync(paymentId, userId);
                return Ok(new { message = "Thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{paymentId}/cancel")]
        public async Task<IActionResult> CancelPayment(int paymentId)
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _paymentService.CancelPaymentAsync(paymentId, userId);
                return Ok(new { message = "Đã hủy vé thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("summary/{paymentId}")]
        public async Task<IActionResult> GetPaymentSummary(int paymentId)
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var summary = await _paymentService.GetPaymentSummaryAsync(paymentId, userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("details/{paymentId}")]
        public async Task<IActionResult> GetPaymentDetails(int paymentId)
        {
            var userId = User.FindFirstValue("userID");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var details = await _paymentService.GetPaymentDetailsAsync(paymentId, userId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
