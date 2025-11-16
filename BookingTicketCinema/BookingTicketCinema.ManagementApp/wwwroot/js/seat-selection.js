document.addEventListener("DOMContentLoaded", function () {

    // 1. Lấy các phần tử
    const seatsContainer = document.querySelector(".seat-map-container");
    const selectedSeatsDisplay = document.getElementById("selected-seats");
    const totalPriceDisplay = document.getElementById("total-price");
    const checkoutButton = document.getElementById("btn-checkout"); // Đây là <button>
    const hiddenSeatIdsInput = document.getElementById("hidden-seat-ids"); // Đây là <input>
    const posForm = document.getElementById("pos-form"); // Đây là <form>

    let selectedSeats = []; // Mảng lưu các ghế đã chọn [{ id, name, price }]

    // 2. Giá vé (Phải khớp với Backend)
    const PRICE_NORMAL = 60000;
    const PRICE_VIP = 70000;
    const PRICE_DOUBLE = 140000;

    if (!seatsContainer || !checkoutButton || !hiddenSeatIdsInput || !posForm) {
        console.error("Lỗi: Thiếu các thành phần DOM (sơ đồ ghế, nút bấm, input ẩn hoặc form).");
        return;
    }

    // 3. Lắng nghe sự kiện click trên TOÀN BỘ sơ đồ
    seatsContainer.addEventListener("click", function (e) {
        // Dùng .closest() để đảm bảo bấm vào <span> bên trong ghế vẫn nhận
        const seat = e.target.closest('.seat');

        // Đảm bảo bấm vào 1 ghế và ghế đó không "disabled" (đã bán)
        if (seat && !seat.classList.contains('disabled')) {

            // Chuyển màu ghế
            seat.classList.toggle('selected');

            // Lấy thông tin từ data-attributes
            const seatId = parseInt(seat.dataset.seatId);
            const seatName = seat.dataset.seatName;
            const seatType = parseInt(seat.dataset.seatType);
            const price = getPrice(seatType);

            const seatIndex = selectedSeats.findIndex(s => s.id === seatId);

            if (seatIndex > -1) {
                // Ghế đã có -> Bỏ chọn (xóa khỏi mảng)
                selectedSeats.splice(seatIndex, 1);
            } else {
                // Ghế mới -> Thêm vào mảng
                selectedSeats.push({ id: seatId, name: seatName, price: price });
            }

            // Cập nhật thanh tóm tắt
            updateSummary();
        }
    });

    // 4. Hàm lấy giá
    function getPrice(type) {
        if (type === 1) return PRICE_VIP;    // 1 = VIP
        if (type === 2) return PRICE_DOUBLE; // 2 = Đôi
        return PRICE_NORMAL; // 0 = Thường
    }

    // 5. Hàm cập nhật thanh tóm tắt (Logic của ManagementApp)
    function updateSummary() {
        if (selectedSeats.length === 0) {
            selectedSeatsDisplay.innerText = "Chưa chọn ghế";
            totalPriceDisplay.innerText = "0đ";
            checkoutButton.classList.add('disabled');
            hiddenSeatIdsInput.value = ""; // Xóa input ẩn
        } else {
            // Sắp xếp ghế theo tên (A1, A2, B1...)
            selectedSeats.sort((a, b) => a.name.localeCompare(b.name, undefined, { numeric: true }));

            const seatNames = selectedSeats.map(s => s.name).join(', ');
            const total = selectedSeats.reduce((sum, s) => sum + s.price, 0);

            selectedSeatsDisplay.innerText = seatNames;
            totalPriceDisplay.innerText = total.toLocaleString('vi-VN') + 'đ';
            checkoutButton.classList.remove('disabled');

            // --- CẬP NHẬT INPUT ẨN (Quan trọng) ---
            // Gán giá trị "1,2,3" vào input để Form Post
            hiddenSeatIdsInput.value = selectedSeats.map(s => s.id).join(',');

            // Phát một sự kiện tùy chỉnh (để báo cho file .cshtml biết)
            // (Code này tôi đã gửi ở tin nhắn trước)
            document.dispatchEvent(new CustomEvent('customSeatUpdate', {
                detail: { ids: selectedSeats.map(s => s.id) }
            }));
        }
    }

    // 6. Thêm logic "Xác nhận" (Confirm) khi Submit Form
    posForm.addEventListener("submit", function (e) {
        if (selectedSeats.length === 0) {
            e.preventDefault(); // Ngăn submit nếu không chọn ghế
            alert("Vui lòng chọn ít nhất 1 ghế.");
            return;
        }

        const total = totalPriceDisplay.innerText;
        const seats = selectedSeatsDisplay.innerText;

        if (!confirm(`Xác nhận Bán vé?\n\nGhế: ${seats}\nTổng tiền: ${total}`)) {
            e.preventDefault(); // Ngăn form gửi đi
        }
        // Nếu bấm OK, form sẽ tự động submit
    });
});