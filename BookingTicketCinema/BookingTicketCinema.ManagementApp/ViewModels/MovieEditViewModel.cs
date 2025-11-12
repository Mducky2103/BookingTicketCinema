using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class MovieEditViewModel
    {
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? TrailerUrl { get; set; }
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Thời lượng là bắt buộc (ví dụ: 02:10:00)")]
        public string Duration { get; set; } = "02:00:00"; // HH:mm:ss

        [Required(ErrorMessage = "Ngày phát hành là bắt buộc (ví dụ: 2025-11-11)")]
        public string ReleaseDate { get; set; } = "yyyy-MM-dd";

        [Required]
        public int Status { get; set; } = 1;

        [Display(Name = "Poster Hiện Tại")]
        public string? CurrentPosterUrl { get; set; }

        [Display(Name = "Tải lên Poster Mới (Tùy chọn)")]
        public IFormFile? PosterFile { get; set; }
    }
}
