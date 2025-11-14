using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class MovieCreateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? TrailerUrl { get; set; }
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Thời lượng là bắt buộc (ví dụ: 02:10:00)")]
        public string Duration { get; set; } = "02:00:00"; // HH:mm:ss

        [Required(ErrorMessage = "Ngày phát hành là bắt buộc (ví dụ: 2025-11-11)")]
        public string ReleaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");

        [Required]
        public int Status { get; set; } = 1; // 1 = NowShowing

        [Display(Name = "Ảnh Poster")]
        public IFormFile? PosterFile { get; set; }

        [Display(Name = "Ảnh Backdrop")]
        public IFormFile? BackdropFile { get; set; }
    }
}
