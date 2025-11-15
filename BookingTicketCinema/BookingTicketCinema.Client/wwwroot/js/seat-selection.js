document.addEventListener("DOMContentLoaded", function () {

    // Lấy các element quan trọng từ trang
    const seatsContainer = document.querySelector(".seat-map-container");
    const selectedSeatsDisplay = document.getElementById("selected-seats");
    const totalPriceDisplay = document.getElementById("total-price");
    const checkoutButton = document.getElementById("btn-checkout");

    // Lấy URL gốc của nút "Tiếp tục" (đã chứa ShowtimeId)
    const originalCheckoutHref = checkoutButton.href;

    // Mảng lưu các ghế đã chọn (dạng { id, name, price })
    let selectedSeats = [];

    // --- GIÁ VÉ (TẠM THỜI) ---
    // Bạn nên lấy giá vé từ API PriceRules,
    // nhưng để đơn giản, chúng ta hardcode giá ở đây.
    const PRICE_NORMAL = 60000;
    const PRICE_VIP = 70000;
    const PRICE_DOUBLE = 140000; // (Ghế đôi)

    // Lắng nghe sự kiện click trên toàn bộ container
    seatsContainer.addEventListener("click", function (e) {

        // Chỉ xử lý nếu click vào một "ghế" (seat)
        if (e.target.classList.contains("seat")) {
            const seat = e.target;

            // Nếu ghế đã bán (occupied), không làm gì cả
            if (seat.classList.contains("seat-occupied")) {
                return;
            }

            // Lấy thông tin từ data-* attributes
            const seatId = parseInt(seat.dataset.seatId);
            const seatName = seat.dataset.seatName;
            const seatType = parseInt(seat.dataset.seatType);

            // Chuyển đổi trạng thái (chọn/bỏ chọn)
            seat.classList.toggle("seat-selected");

            if (seat.classList.contains("seat-selected")) {
                // --- CHỌN GHẾ ---
                let price = PRICE_NORMAL;
                if (seatType === 1) price = PRICE_VIP;
                if (seatType === 2) price = PRICE_DOUBLE;

                selectedSeats.push({ id: seatId, name: seatName, price: price });
            } else {
                // --- BỎ CHỌN GHẾ ---
                selectedSeats = selectedSeats.filter(s => s.id !== seatId);
            }

            // Cập nhật lại thanh tóm tắt
            updateSummary();
        }
    });

    function updateSummary() {
        if (selectedSeats.length === 0) {
            selectedSeatsDisplay.innerText = "Chưa chọn ghế";
            totalPriceDisplay.innerText = "0đ";
            checkoutButton.classList.add("disabled");
            checkoutButton.href = "#"; // Vô hiệu hóa link
        } else {
            // Cập nhật tên ghế
            selectedSeatsDisplay.innerText = selectedSeats.map(s => s.name).join(", ");

            // Tính tổng tiền
            let total = selectedSeats.reduce((sum, s) => sum + s.price, 0);
            totalPriceDisplay.innerText = total.toLocaleString('vi-VN') + 'đ';

            // Kích hoạt nút
            checkoutButton.classList.remove("disabled");

            // Tạo query string cho các ghế đã chọn (ví dụ: "1,2,3")
            let seatIdsQuery = selectedSeats.map(s => s.id).join(',');

            // Gán link cho nút (mang theo ds ghế đã chọn)
            // URL gốc đã chứa showtimeId
            checkoutButton.href = `${originalCheckoutHref}&seatIds=${seatIdsQuery}`;
        }
    }
});