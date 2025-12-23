namespace CarRentalMVC.Models
{
    public class VehicleBrandViewModel
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
