namespace CarRentalMVC.Models
{
    public enum RentalStatus
    {
        Pending = 0,       // Đang chờ duyệt
        Processing = 1,    // Đang xử lý
        Delivering = 2,    // Đang giao xe
        WaitingUser = 3,   // Đợi người dùng nhận xe
        Completed = 4,     // Hoàn tất
        Cancelled = 5,     // User hủy
        Rejected = 6       // Admin từ chối
    }
}
