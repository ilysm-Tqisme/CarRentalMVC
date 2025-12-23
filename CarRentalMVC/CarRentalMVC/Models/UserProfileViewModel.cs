using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class UserProfileViewModel
    {
        public int UserID { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string? FullName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? ProfileImage { get; set; } = "/images/avatar.jpg";

        [Display(Name = "CMND/CCCD")]
        public string? IDCardImage { get; set; }

        [Display(Name = "Bằng lái xe")]
        public string? DriverLicenseImage { get; set; }
    }

    //public class RentalHistoryViewModel
    //{
    //    public int RentalId { get; set; }
    //    public string CarName { get; set; } = string.Empty;
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //    public decimal TotalPrice { get; set; }
    //    public string Status { get; set; } = string.Empty;
    //}

    public class AvatarViewModel
    {
        public string CurrentAvatar { get; set; } = "/images/avatar.jpg";
        public IFormFile? AvatarFile { get; set; }
    }
}
