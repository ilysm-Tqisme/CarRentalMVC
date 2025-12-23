using System.ComponentModel.DataAnnotations;

namespace CarRentalMVC.Models
{
    public class RentalViewModel
    {
        public int Id { get; set; }
        public int RentalID => Id; // tự map qua để Razor vẫn dùng được


        public int VehicleID { get; set; }
        public int UserID { get; set; }
        public int BranchID { get; set; }

        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Active, Completed, Cancelled
        public string? RejectionReason { get; set; }
        public DateTime CreatedDate { get; set; }

        // Thông tin chi tiết
        public string VehicleName { get; set; }
        public string VehicleImage { get; set; }
        public decimal DailyRate { get; set; }
        public int Seats { get; set; }
        public string BrandName { get; set; }
        public string TypeName { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
    }

    public class CreateRentalViewModel
    {
        public int VehicleID { get; set; }
        public int BranchID { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Display info
        public string VehicleName { get; set; }
        public string VehicleImage { get; set; }
        public decimal DailyRate { get; set; }
        public int Seats { get; set; }
        public string BrandName { get; set; }
        public string TypeName { get; set; }
    }

    public class CalculatePriceViewModel
    {
        public int VehicleID { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }

    public class PriceResponseViewModel
    {
        public decimal DailyRate { get; set; }
        public int Days { get; set; }
        public decimal TotalPrice { get; set; }
    }

    //public class RentalHistoryViewModel
    //{
    //    public int RentalID { get; set; }
    //    public string VehicleName { get; set; }
    //    public string VehicleImage { get; set; }
    //    public DateTime RentalDate { get; set; }
    //    public DateTime ReturnDate { get; set; }
    //    public decimal TotalPrice { get; set; }
    //    public string Status { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //}

    public class ApproveRentalViewModel
    {
        public int RentalID { get; set; }
    }

    public class RejectRentalViewModel
    {
        public int RentalID { get; set; }
        public string RejectionReason { get; set; }
    }

    public class CancelRentalViewModel
    {
        public int RentalID { get; set; }
    }

    public class RentalStatisticsViewModel
    {
        public int TotalRentals { get; set; }
        public int PendingRentals { get; set; }
        public int ApprovedRentals { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<RentalViewModel> RecentRentals { get; set; }
    }
}
