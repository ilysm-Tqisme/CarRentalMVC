using Microsoft.AspNetCore.Mvc;

namespace CarRentalMVC.ViewComponents
{
    /// <summary>
    /// ViewComponent hiển thị trạng thái người dùng (Khóa/Mở)
    /// Sử dụng: @await Component.InvokeAsync("UserStatus", new { isLocked = user.IsLocked })
    /// </summary>
    public class UserStatusViewComponent : ViewComponent
    {
        /// <summary>
        /// Render ViewComponent hiển thị trạng thái người dùng
        /// </summary>
        public IViewComponentResult Invoke(bool isLocked)
        {
            // Truyền dữ liệu vào View
            ViewBag.IsLocked = isLocked;
            return View();
        }
    }
}

