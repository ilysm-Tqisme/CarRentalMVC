using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class VehicleTypeViewModel
    {
        public int? TypeID { get; set; }

        [Required(ErrorMessage = "Tên loại xe không được để trống!")]
        [Display(Name = "Tên loại xe")]
        public string TypeName { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }
}
