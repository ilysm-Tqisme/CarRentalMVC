namespace CarRentalMVC.Models
{
    /// <summary>
    /// ViewModel cho quản lý người dùng - Admin
    /// </summary>
    public class UserManagementViewModel
    {
        /// <summary>
        /// DTO hiển thị danh sách người dùng
        /// </summary>
        public class UserListViewModel
        {
            public int UserID { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string Role { get; set; } = "User";
            public bool IsActive { get; set; } = true;
            public bool IsLocked { get; set; } = false;
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        /// <summary>
        /// ViewModel cho trang quản lý người dùng
        /// </summary>
        public class UserManagementIndexViewModel
        {
            public List<UserListViewModel> Users { get; set; } = new List<UserListViewModel>();
            public string? Message { get; set; }
            public string? Error { get; set; }
        }
    }
}

