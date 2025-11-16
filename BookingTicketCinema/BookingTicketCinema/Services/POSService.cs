using BookingTicketCinema.DTO.Booking;
using BookingTicketCinema.DTO.Payment;
using BookingTicketCinema.DTO.POS;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;
using static BookingTicketCinema.Models.Payment;
using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.Services
{
    public class POSService : IPOSService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IShowtimeRepository _showtimeRepository;

        private const decimal PRICE_NORMAL = 60000;
        private const decimal PRICE_VIP = 70000;
        private const decimal PRICE_DOUBLE = 140000;

        public POSService(
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

        public async Task<PaymentResponseDto> CreateBookingAtCounterAsync(POSRequestDto dto, string staffUserId)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(dto.ShowtimeId);
            if (showtime == null || showtime.StartTime < DateTime.Now)
                throw new Exception("Suất chiếu không hợp lệ hoặc đã bắt đầu.");

            // DÙNG TRANSACTION (Rất quan trọng)
            // Chúng ta phải đảm bảo không ai đặt vé online (Pending)
            // cùng lúc staff đang bán vé (Paid)

            // (Giả định DbContext của bạn được cấu hình để tự động
            // dùng Transaction khi SaveChangesAsync. Nếu không,
            // bạn cần bọc logic này trong 1 Transaction)

            var takenTickets = await _ticketRepository.GetByShowtimeIdAsync(dto.ShowtimeId);
            var takenSeatIds = takenTickets.Select(t => t.SeatId).ToHashSet();

            if (dto.SeatIds.Any(id => takenSeatIds.Contains(id)))
            {
                throw new Exception("Ghế vừa được người khác chọn. Vui lòng tải lại sơ đồ ghế.");
            }

            // Lấy thông tin ghế để Tính tiền
            var seats = await _seatRepository.GetByIdsAsync(dto.SeatIds);
            decimal totalAmount = seats.Sum(seat => CalculatePrice(seat));

            // Tạo Đơn hàng (Payment) - TRẠNG THÁI COMPLETED
            var payment = new Payment
            {
                Amount = totalAmount,
                Method = dto.Method, 
                Status = PaymentStatus.Completed, 
                UserId = staffUserId, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Tạo các Vé (Ticket) - TRẠNG THÁI PAID
            var tickets = new List<Ticket>();
            foreach (var seat in seats)
            {
                tickets.Add(new Ticket
                {
                    ShowtimeId = dto.ShowtimeId,
                    SeatId = seat.SeatId,
                    UserId = staffUserId, 
                    Status = TicketStatus.Paid, 
                    Payment = payment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            payment.Tickets = tickets;

            var createdPayment = await _paymentRepository.CreateAsync(payment);

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
            int seatType = (int)(seat.SeatGroup?.Type ?? SeatGroup.SeatType.Standard);
            switch (seatType)
            {
                case 1: return PRICE_VIP;
                case 2: return PRICE_DOUBLE;
                default: return PRICE_NORMAL;
            }
        }

        public async Task<PaymentResponseDto> GetReceiptAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new Exception("Không tìm thấy hóa đơn.");

            var firstTicket = payment.Tickets.FirstOrDefault();
            if (firstTicket == null)
                throw new Exception("Hóa đơn này không có vé.");

            var showtime = await _showtimeRepository.GetByIdAsync(firstTicket.ShowtimeId);
            var seats = await _seatRepository.GetByIdsAsync(payment.Tickets.Select(t => t.SeatId).ToList());

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

        public async Task<PagedResultDto<PaymentHistoryDto>> GetMyBookingHistoryAsync(string staffUserId, int pageNumber, int pageSize)
        {
            var pagedPayments = await _paymentRepository.GetByUserIdPagedAsync(staffUserId, pageNumber, pageSize);

            var totalCount = await _paymentRepository.GetCountByUserIdAsync(staffUserId);

            var paymentDtos = new List<PaymentHistoryDto>();
            foreach (var payment in pagedPayments)
            {
                var firstTicket = payment.Tickets.FirstOrDefault();
                if (firstTicket == null) continue;

                var dto = new PaymentHistoryDto
                {
                    PaymentId = payment.PaymentId,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    CreatedAt = payment.CreatedAt,
                    MovieTitle = firstTicket.Showtime?.Movie?.Title ?? "N/A",
                    PosterUrl = firstTicket.Showtime?.Movie?.PosterUrl,
                    Showtime = firstTicket.Showtime.StartTime,
                    RoomName = firstTicket.Showtime?.Room?.Name ?? "N/A",
                    TicketsInPayment = payment.Tickets.Select(t => new TicketHistoryDto
                    {
                        TicketId = t.TicketId,
                        PaymentId = t.PaymentId ?? 0,
                        Seat = t.Seat?.SeatNumber ?? "N/A",
                        Status = t.Status,
                    }).ToList()
                };
                paymentDtos.Add(dto);
            }

            return new PagedResultDto<PaymentHistoryDto>
            {
                Items = paymentDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
