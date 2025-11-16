using BookingTicketCinema.Data;
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
        private readonly IPromotionRepository _promotionRepository;
        private readonly IVoucherRedemptionRepository _redemptionRepository;
        private readonly CinemaDbContext _context;

        private const decimal PRICE_NORMAL = 60000;
        private const decimal PRICE_VIP = 70000;
        private const decimal PRICE_DOUBLE = 140000;
        public PaymentService(
            IPaymentRepository paymentRepository,
            ITicketRepository ticketRepository,
            ISeatRepository seatRepository,
            IShowtimeRepository showtimeRepository,
            IPromotionRepository promotionRepository,
            IVoucherRedemptionRepository redemptionRepository,
            CinemaDbContext context)
        {
            _paymentRepository = paymentRepository;
            _ticketRepository = ticketRepository;
            _seatRepository = seatRepository;
            _showtimeRepository = showtimeRepository;
            _promotionRepository = promotionRepository; 
            _redemptionRepository = redemptionRepository; 
            _context = context; 
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

            payment.Status = PaymentStatus.Completed;
            payment.UpdatedAt = DateTime.UtcNow;

            foreach (var ticket in payment.Tickets)
            {
                ticket.Status = TicketStatus.Paid;
                ticket.UpdatedAt = DateTime.UtcNow;
            }
            if (payment.PromotionId.HasValue)
            {
                // Kiểm tra lần cuối (phòng trường hợp User mở 2 tab)
                if (await _redemptionRepository.HasUserRedeemedAsync(payment.PromotionId.Value, userId))
                {
                    // (Tình huống hiếm gặp: User thanh toán 2 đơn cùng lúc)
                    throw new Exception("Lỗi: Mã khuyến mãi đã được sử dụng ở một giao dịch khác.");
                }

                var redemption = new VoucherRedemption
                {
                    PromotionId = payment.PromotionId.Value,
                    UserId = userId,
                    UsedAt = DateTime.UtcNow
                };
                // Thêm vào context để SaveChangesAsync
                await _redemptionRepository.AddAsync(redemption);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPaymentAsync(int paymentId, string userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null || payment.UserId != userId)
                throw new Exception("Không tìm thấy đơn hàng.");

            // Đơn hàng đã thất bại hoặc bị hủy thì không thể hủy lại
            if (payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                throw new Exception("Đơn hàng này đã được hủy trước đó.");

            //Đơn hàng đã hoàn thành thì không thể hủy
            if (payment.Status == PaymentStatus.Completed)
            {
                // (Sau này bạn có thể thêm logic check Giờ chiếu ở đây)
                throw new Exception("Không thể hủy đơn hàng đã thanh toán thành công.");
            }
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
        public async Task<PaymentResponseDto> GetPaymentSummaryAsync(int paymentId, string userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null || payment.UserId != userId)
                throw new Exception("Không tìm thấy đơn hàng.");

            if (payment.Status != PaymentStatus.Pending)
                throw new Exception("Đơn hàng này không còn chờ thanh toán.");

            var tickets = payment.Tickets;
            var showtime = await _showtimeRepository.GetByIdAsync(tickets.First().ShowtimeId);
            var seats = await _seatRepository.GetByIdsAsync(tickets.Select(t => t.SeatId).ToList());

            // Map sang DTO
            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt,
                MovieTitle = showtime.Movie.Title,
                RoomName = showtime.Room.Name,
                Showtime = showtime.StartTime,
                SeatNumbers = seats.Select(s => s.SeatNumber).ToList()
            };
        }
        public async Task<PaymentResponseDto> GetPaymentDetailsAsync(int paymentId, string userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null || payment.UserId != userId)
                throw new Exception("Không tìm thấy đơn hàng.");

            var tickets = payment.Tickets;
            var firstTicket = tickets.FirstOrDefault();
            if (firstTicket == null)
                throw new Exception("Đơn hàng không có vé.");

            var showtime = await _showtimeRepository.GetByIdAsync(firstTicket.ShowtimeId);
            var seats = await _seatRepository.GetByIdsAsync(tickets.Select(t => t.SeatId).ToList());

            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                Status = payment.Status, 
                CreatedAt = payment.CreatedAt,
                MovieTitle = showtime.Movie.Title,
                RoomName = showtime.Room.Name,
                Showtime = showtime.StartTime,
                SeatNumbers = seats.Select(s => s.SeatNumber).ToList()
            };
        }

        public async Task<PaymentResponseDto> ApplyVoucherAsync(int paymentId, string code, string userId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null || payment.UserId != userId)
                throw new Exception("Không tìm thấy đơn hàng.");
            if (payment.Status != Payment.PaymentStatus.Pending)
                throw new Exception("Đơn hàng không còn ở trạng thái chờ.");

            var promotion = await _promotionRepository.GetByCodeAsync(code);

            // --- Logic kiểm tra mã ---
            if (promotion == null)
                throw new Exception("Mã khuyến mãi không tồn tại.");
            if (!promotion.IsActive)
                throw new Exception("Mã khuyến mãi đã bị vô hiệu hóa.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today < promotion.StartDate || today > promotion.EndDate)
                throw new Exception("Mã khuyến mãi không nằm trong thời gian áp dụng.");

            if (await _redemptionRepository.HasUserRedeemedAsync(promotion.PromotionId, userId))
                throw new Exception("Mã khuyến mãi này đã được bạn sử dụng.");

            // --- Tính toán lại giá ---
            // 1. Lấy giá gốc (phòng trường hợp áp dụng 2 lần)
            var seats = await _seatRepository.GetByIdsAsync(payment.Tickets.Select(t => t.SeatId).ToList());
            decimal originalAmount = seats.Sum(seat => CalculatePrice(seat));

            // 2. Tính tiền sau giảm giá
            decimal discount = originalAmount * promotion.DiscountPercent;
            decimal finalAmount = originalAmount - discount;

            // 3. Cập nhật Payment
            payment.Amount = finalAmount;
            payment.PromotionId = promotion.PromotionId; // (Lưu tạm)

            await _paymentRepository.UpdateAsync(payment);

            // 4. Trả về DTO mới
            // (Gọi lại GetPaymentSummaryAsync để lấy DTO đã map)
            return await GetPaymentSummaryAsync(paymentId, userId);
        }
    }
}
