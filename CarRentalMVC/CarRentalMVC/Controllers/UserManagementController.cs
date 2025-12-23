using CarRentalMVC.Helps;
using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace CarRentalMVC.Controllers
{
    /// <summary>
    /// Controller quản lý người dùng - Chỉ Admin mới có quyền truy cập
    /// </summary>
    public class UserManagementController : Controller
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _context;

        /// <summary>
        /// Constructor - Inject ApiService và IHttpContextAccessor
        /// </summary>
        public UserManagementController(ApiService api, IHttpContextAccessor context)
        {
            _api = api;
            _context = context;
        }

        /// <summary>
        /// Kiểm tra người dùng hiện tại có phải Admin không
        /// </summary>
        private bool IsAdmin()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return false;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value;

            return role == "Admin";
        }

        /// <summary>
        /// Hiển thị danh sách tất cả người dùng (Admin only)
        /// GET: UserManagement
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // Kiểm tra quyền Admin
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                // Lấy danh sách người dùng từ API
                var response = await _api.GetAsync("api/Users", token);
                var users = JsonConvert.DeserializeObject<List<UserManagementViewModel.UserListViewModel>>(response) 
                    ?? new List<UserManagementViewModel.UserListViewModel>();

                var model = new UserManagementViewModel.UserManagementIndexViewModel
                {
                    Users = users,
                    Message = TempData["Message"]?.ToString(),
                    Error = TempData["Error"]?.ToString()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                var model = new UserManagementViewModel.UserManagementIndexViewModel
                {
                    Users = new List<UserManagementViewModel.UserListViewModel>(),
                    Error = $"Lỗi khi tải danh sách người dùng: {ex.Message}"
                };
                return View(model);
            }
        }

        /// <summary>
        /// Khóa tài khoản người dùng (Admin only)
        /// PUT: UserManagement/Lock/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Lock(int id)
        {
            // Kiểm tra đăng nhập
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // Kiểm tra quyền Admin
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                // Gọi API để khóa tài khoản
                var response = await _api.PutAsync($"api/Users/{id}/lock", new { }, token);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                TempData["Message"] = result?.message?.ToString() ?? "Đã khóa tài khoản thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi khóa tài khoản: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Mở khóa tài khoản người dùng (Admin only)
        /// PUT: UserManagement/Unlock/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Unlock(int id)
        {
            // Kiểm tra đăng nhập
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // Kiểm tra quyền Admin
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                // Gọi API để mở khóa tài khoản
                var response = await _api.PutAsync($"api/Users/{id}/unlock", new { }, token);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                TempData["Message"] = result?.message?.ToString() ?? "Đã mở khóa tài khoản thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi mở khóa tài khoản: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Toggle trạng thái khóa/mở tài khoản (Admin only)
        /// PUT: UserManagement/ToggleLock/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleLock(int id, bool isLocked)
        {
            // Kiểm tra đăng nhập
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // Kiểm tra quyền Admin
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            try
            {
                // Gọi API để toggle trạng thái khóa/mở
                var request = new { IsLocked = isLocked };
                var response = await _api.PutAsync($"api/Users/{id}/toggle-lock", request, token);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                TempData["Message"] = result?.message?.ToString() ?? "Đã cập nhật trạng thái tài khoản thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật trạng thái tài khoản: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

