using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class RentalCreateViewModel
    {
        public int VehicleID { get; set; }
        public string VehicleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh nhận xe")]
        public int BranchPickupID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh trả xe")]
        public int BranchReturnID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime EndDateTime { get; set; }

        [Display(Name = "Giá thuê / ngày")]
        public decimal PricePerDay { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal FinalPrice { get; set; }

        public string PaymentMethod { get; set; } = "Cash";
    }
}
