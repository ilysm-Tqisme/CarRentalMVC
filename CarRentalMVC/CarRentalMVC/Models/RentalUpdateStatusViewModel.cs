using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class RentalUpdateStatusViewModel
    {
        [Required]
        public int RentalID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Lý do hủy (nếu có)")]
        public string? CancellationReason { get; set; }
    }
}
