using BookingTicketCinema.Data;
using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly CinemaDbContext _context;
        public PaymentController(CinemaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Giả lập tạo payment cho 1 nhóm ticket.
        /// - Nếu Ticket đã có trạng thái Paid thì sẽ bỏ qua.
        /// - Mỗi Ticket sẽ được tạo 1 Payment (1:1) và ticket.Status -> Paid.
        /// </summary>
        [HttpPost("simulate")]
        
        public async Task<IActionResult> SimulatePayment([FromBody] CreatePaymentDTO dto)
        {
            if (dto == null || dto.TicketIds == null || !dto.TicketIds.Any())
                return BadRequest(new { message = "TicketIds required." });

            // Lấy tickets
            var tickets = await _context.Tickets
                .Where(t => dto.TicketIds.Contains(t.TicketId))
                .ToListAsync();

            if (tickets.Count != dto.TicketIds.Count)
                return NotFound(new { message = "Một hoặc nhiều ticket không tồn tại." });

            // Tính chia đều amount cho mỗi ticket (nếu amount được truyền)
            decimal amountPerTicket = dto.Amount;
            if (dto.Amount <= 0)
            {
                // nếu không truyền amount, cố gắng tính mặc định (nếu bạn có logic giá riêng, tích hợp ở đây)
                amountPerTicket = 0m;
            }
            else
            {
                amountPerTicket = Math.Round(dto.Amount / tickets.Count, 2);
            }

            var createdPayments = new List<PaymentResponseDTO>();

            foreach (var t in tickets)
            {
                if (t.Status == Ticket.TicketStatus.Paid)
                {
                    // Nếu đã paid: trả về thông tin và không tạo payment mới
                    createdPayments.Add(new PaymentResponseDTO
                    {
                        TicketId = t.TicketId,
                        PaymentId = t.Payment?.PaymentId ?? 0,
                        Amount = t.Payment?.Amount ?? 0m,
                        Status = t.Payment != null ? (PaymentStatusEnum)t.Payment.Status : PaymentStatusEnum.Completed
                    });
                    continue;
                }

                var payment = new Payment
                {
                    Amount = amountPerTicket,
                    Method = dto.Method,
                    Status = Payment.PaymentStatus.Completed, // vì đây là simulate: giả lập thanh toán thành công
                    CreatedAt = DateTime.UtcNow,
                    TicketId = t.TicketId
                };

                // thêm payment
                _context.Add(payment);

                // cập nhật ticket
                t.Payment = payment;
                t.Status = Ticket.TicketStatus.Paid;
                t.UpdatedAt = DateTime.UtcNow;

                createdPayments.Add(new PaymentResponseDTO
                {
                    TicketId = t.TicketId,
                    PaymentId = 0, // sẽ có sau khi SaveChanges
                    Amount = payment.Amount,
                    Status = PaymentStatusEnum.Completed
                });
            }

            await _context.SaveChangesAsync();

            // cập nhật PaymentId trả về
            var paymentIds = createdPayments
                .Select(pdto =>
                {
                    var p = _context.Payments.FirstOrDefault(x => x.TicketId == pdto.TicketId);
                    if (p != null) pdto.PaymentId = p.PaymentId;
                    return pdto;
                })
                .ToList();

            return Ok(new
            {
                message = "Simulated payment completed.",
                payments = paymentIds
            });
        }

        /// <summary>
        /// Confirm payment (dùng nếu bạn muốn endpoint riêng để xác nhận/ webhooks)
        /// - Bổ sung: chuyển status Payment -> Completed và Ticket -> Paid
        /// </summary>
        [HttpPost("confirm/{paymentId:int}")]
        
        public async Task<IActionResult> ConfirmPayment([FromRoute] int paymentId)
        {
            var payment = await _context.Set<Payment>()
                .Include(p => p.Ticket)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return NotFound(new { message = "Payment not found." });

            // Set trạng thái
            payment.Status = Payment.PaymentStatus.Completed;
            payment.UpdatedAt = DateTime.UtcNow;

            if (payment.Ticket != null)
            {
                payment.Ticket.Status = Ticket.TicketStatus.Paid;
                payment.Ticket.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment confirmed and ticket(s) marked as Paid.", paymentId = payment.PaymentId });
        }

        /// <summary>
        /// Lấy trạng thái payment
        /// </summary>
        [HttpGet("{paymentId:int}")]
        
        public async Task<IActionResult> GetPayment(int paymentId)
        {
            var payment = await _context.Set<Payment>()
                .Include(p => p.Ticket)
                .Where(p => p.PaymentId == paymentId)
                .Select(p => new
                {
                    p.PaymentId,
                    p.Amount,
                    p.Method,
                    Status = p.Status,
                    TicketId = p.TicketId,
                    TicketStatus = p.Ticket != null ? p.Ticket.Status : (Ticket.TicketStatus?)null,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (payment == null) return NotFound();
            return Ok(payment);
        }
    }

}
