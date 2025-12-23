using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class BranchViewModel
    {
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Tên chi nhánh là bắt buộc")]
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(10, ErrorMessage = "Số điện thoại không hợp lệ", MinimumLength = 9)]
        public string Phone { get; set; }

        public string? Description { get; set; }
        public string? BranchImage { get; set; }
        public bool IsActive { get; set; } = true;

        public IFormFile? ImageFile { get; set; }
    }

    // Dùng riêng khi tạo
    public class BranchCreateModel
    {
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
