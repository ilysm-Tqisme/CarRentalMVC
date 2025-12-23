namespace CarRentalMVC.Models
{
    public class RentalHistoryViewModel
    {
        public int RentalID { get; set; }
        public string VehicleName { get; set; }
        public string VehicleImage { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
