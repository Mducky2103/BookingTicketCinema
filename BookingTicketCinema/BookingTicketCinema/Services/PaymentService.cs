using BookingTicketCinema.DTO.Payment;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;
using static BookingTicketCinema.Models.Payment;
using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository; // (Từ TicketController)
        private readonly ISeatRepository _seatRepository;     // (Từ SeatEndpoints)
        private readonly IShowtimeRepository _showtimeRepository; // (Từ ShowtimeEndpoints)

        private const decimal PRICE_NORMAL = 60000;
        private const decimal PRICE_VIP = 70000;
        private const decimal PRICE_DOUBLE = 140000;
        public PaymentService(
            IPaymentRepository paymentRepository,
            ITicketRepository ticketRepository,
            ISeatRepository seatRepository,
            IShowtimeRepository showtimeRepository)
        {
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _showtimeRepository = showtimeRepository;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto dto, string userId)
        {
            // 1. Kiểm tra Suất chiếu
            var showtime = await _showtimeRepository.GetByIdAsync(dto.ShowtimeId);
            if (showtime == null)
                throw new Exception("Suất chiếu không hợp lệ.");
            if (showtime.StartTime < DateTime.Now)
                throw new Exception("Suất chiếu này đã bắt đầu.");

            // 2. Kiểm tra Ghế đã bán (Logic từ TicketService)
            var takenTickets = await _ticketRepository.GetByShowtimeIdAsync(dto.ShowtimeId);
            var takenSeatIds = takenTickets.Select(t => t.SeatId).ToHashSet();

            if (dto.SeatIds.Any(id => takenSeatIds.Contains(id)))
            {
                throw new Exception("Ghế đã được người khác chọn. Vui lòng quay lại.");
            }

            // 3. Lấy thông tin ghế để Tính tiền
            var seats = await _seatRepository.GetByIdsAsync(dto.SeatIds);
            decimal totalAmount = 0;
            foreach (var seat in seats)
            {
                totalAmount += CalculatePrice(seat);
            }

            // 4. Tạo Đơn hàng (Payment)
            var payment = new Payment
            {
                Amount = totalAmount,
                Method = PaymentMethod.Online, // (Tạm thời, sau này lấy từ DTO)
                Status = PaymentStatus.Pending, // (Chờ thanh toán)
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 5. Tạo các Vé (Ticket)
            var tickets = new List<Ticket>();
            foreach (var seat in seats)
            {
                tickets.Add(new Ticket
                {
                    ShowtimeId = dto.ShowtimeId,
                    SeatId = seat.SeatId,
                    UserId = userId,
                    Status = TicketStatus.Reserved, // (Vé đang chờ thanh toán)
                    //Payment = payment, // Gán vé này vào Payment
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            payment.Tickets = tickets;

            // 6. Lưu vào DB
            var createdPayment = await _paymentRepository.CreateAsync(payment);

            // 7. Trả DTO về cho Client
            return new PaymentResponseDto
            {
                PaymentId = createdPayment.PaymentId,
                Amount = createdPayment.Amount,
                Status = createdPayment.Status,
                CreatedAt = createdPayment.CreatedAt,
                MovieTitle = showtime.Movie.Title,
                RoomName = showtime.Room.Name,
                Showtime = showtime.StartTime,
                SeatNumbers = seats.Select(s => s.SeatNumber).ToList()
            };
        }

        private decimal CalculatePrice(Seat seat)
        {
            // Logic dựa trên SeatType (từ SeatGroup)
            int seatType = (int)(seat.SeatGroup?.Type ?? SeatGroup.SeatType.Standard);

            switch (seatType)
            {
                case 1: // VIP
                    return PRICE_VIP;
                case 2: // Đôi
                    return PRICE_DOUBLE;
                case 0: // Thường
                default:
                    return PRICE_NORMAL;
            }
        }

        public async Task<bool> ConfirmPaymentAsync(int paymentId, string userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            // Kiểm tra bảo mật
            if (payment == null || payment.UserId != userId)
                throw new Exception("Không tìm thấy đơn hàng.");

            if (payment.Status != PaymentStatus.Pending)
                throw new Exception("Đơn hàng này đã được xử lý.");

            // Cập nhật trạng thái (Fake payment success)
            payment.Status = PaymentStatus.Completed;
            payment.UpdatedAt = DateTime.UtcNow;

            foreach (var ticket in payment.Tickets)
            {
                ticket.Status = TicketStatus.Paid;
                ticket.UpdatedAt = DateTime.UtcNow;
            }

            await _paymentRepository.UpdateAsync(payment);
            return true;
        }

        public async Task<bool> CancelPaymentAsync(int paymentId, string userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            // Kiểm tra bảo mật
            if (payment == null || payment.UserId != userId)
                throw new Exception("Không tìm thấy đơn hàng.");

            // Chỉ hủy đơn đang chờ hoặc đã thanh toán
            if (payment.Status == PaymentStatus.Failed)
                throw new Exception("Đơn hàng này đã được hủy trước đó.");

            payment.Status = PaymentStatus.Failed;
            payment.UpdatedAt = DateTime.UtcNow;

            foreach (var ticket in payment.Tickets)
            {
                ticket.Status = TicketStatus.Cancelled;
                ticket.UpdatedAt = DateTime.UtcNow;
            }

            await _paymentRepository.UpdateAsync(payment);
            return true;
        }
    }
}
