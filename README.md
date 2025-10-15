# BookingTicketCinema
Web đặt vé xem phim
1. Mục tiêu hệ thống
  - Xây dựng Web API cho hệ thống đặt vé xem phim tại một rạp chiếu phim duy nhất.
  - Người dùng có thể xem phim đang chiếu, xem lịch chiếu, chọn suất chiếu, đặt vé, thanh toán, và quản lý vé đã mua.
  - Admin có thể quản lý phim, suất chiếu, phòng chiếu, và kiểm soát người dùng.
2. Vai trò người dùng (Roles)
  - 2.1. Khách (Guest)
      - Xem danh sách phim đang chiếu & sắp chiếu
      - Xem thông tin chi tiết phim, trailer
      - Xem lịch chiếu của từng phim
      - Đăng ký / Đăng nhập
  - 2.2. Khách hàng (Customer/User)
      - Tất cả quyền của Guest
      - Đặt vé xem phim
      - Chọn chỗ ngồi
      - Thanh toán (giả lập)
      - Xem lại lịch sử đặt vé
      - Hủy vé (nếu chưa đến suất chiếu)
  - 2.3. Nhân viên (Staff)
      - Quản lý lịch chiếu, phòng chiếu, và vé
      - Kiểm tra vé hợp lệ tại rạp
      - Cập nhật trạng thái vé (đã sử dụng, hủy…)
  - 2.4. Quản trị viên (Admin)
      - Quản lý phim (CRUD: thêm/sửa/xóa phim, upload poster, trailer)
      - Quản lý nhân viên, tài khoản khách hàng
      - Quản lý phòng chiếu và suất chiếu
      - Xem thống kê doanh thu theo phim, suất chiếu, ngày
3. Các chức năng chính
  - 3.1. Người dùng
      - Đăng ký, đăng nhập (JWT Authentication)
      - Cập nhật hồ sơ cá nhân
      - Xem danh sách phim đang chiếu
      - Xem chi tiết phim (mô tả, thời lượng, trailer, diễn viên,…)
      - Xem lịch chiếu từng phim theo ngày
      - Chọn suất chiếu, chọn ghế, đặt vé
      - Thanh toán vé (tích hợp giả lập)
      - Xem lịch sử đặt vé và trạng thái
  - 3.2. Admin / Staff
      - CRUD phim, lịch chiếu, phòng chiếu
      - Quản lý người dùng
      - Kiểm soát vé, cập nhật trạng thái vé
      - Thống kê doanh thu, lượng đặt vé

