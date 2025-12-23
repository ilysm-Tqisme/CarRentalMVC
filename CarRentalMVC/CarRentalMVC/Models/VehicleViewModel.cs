using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class VehicleViewModel
    {
        public int VehicleID { get; set; }
        public string VehicleName { get; set; } = "";
        public int BrandID { get; set; }
        public int TypeID { get; set; }
        public int BranchID { get; set; }
        public string? BrandName { get; set; }
        public string? TypeName { get; set; }
        public string? BranchName { get; set; }

        public decimal PricePerDay { get; set; }
        public int Seats { get; set; }
        public string? Description { get; set; }
        public string? VehicleImage { get; set; }
        public string? Status { get; set; } = "Available";
        public bool IsActive { get; set; } = true;

        // 🔹 File upload (chọn ảnh)
        public IFormFile? ImageFile { get; set; }

        // 🔹 Danh sách dropdown
        public List<BrandViewModel>? Brands { get; set; }
        public List<TypeViewModel>? Types { get; set; }
        public List<BranchViewModel>? Branches { get; set; }
    }

    public class BrandViewModel
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; } = "";
    }

    public class TypeViewModel
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; } = "";
    }
}

