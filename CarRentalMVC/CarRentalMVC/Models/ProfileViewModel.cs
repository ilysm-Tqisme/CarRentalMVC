namespace CarRentalMVC.Models
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public string? ProfileImage { get; set; } = "/images/default-avatar.png";
  
        public string? IdCardImage { get; set; }
        public string? DriverLicenseImage { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
