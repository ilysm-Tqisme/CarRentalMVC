using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace CarRentalMVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _context;

        public ProfileController(ApiService api, IHttpContextAccessor context)
        {
            _api = api;
            _context = context;
        }

        /// <summary>
        /// Xem hồ sơ cá nhân
        /// Lấy thông tin người dùng từ API và hiển thị
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = _context.HttpContext!.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Account");

                // Lấy userId từ JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    ViewBag.Error = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                // Lấy thông tin hồ sơ từ API
                var response = await _api.GetAsync($"api/Profile/{userId}", token);
                var user = JsonConvert.DeserializeObject<UserProfileViewModel>(response);
                
                if (user == null)
                {
                    ViewBag.Error = "Không tải được dữ liệu hồ sơ người dùng.";
                    return View(new UserProfileViewModel());
                }
                
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi tải hồ sơ: {ex.Message}";
                return View(new UserProfileViewModel());
            }
        }

        // 🟢 Hiển thị form chỉnh sửa hồ sơ
        public async Task<IActionResult> Edit()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            var response = await _api.GetAsync($"api/Profile/{userId}", token);
            var user = JsonConvert.DeserializeObject<UserProfileViewModel>(response);
            return View(user);
        }

        // 🟢 Cập nhật hồ sơ cá nhân
        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileViewModel model, IFormFile? ProfileImage, IFormFile? IDCardImage, IFormFile? DriverLicenseImage)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.FullName ?? ""), "FullName");
            formData.Add(new StringContent(model.Address ?? ""), "Address");
            formData.Add(new StringContent(model.Phone ?? ""), "Phone");

            if (ProfileImage != null)
                formData.Add(new StreamContent(ProfileImage.OpenReadStream()), "ProfileImage", ProfileImage.FileName);

            if (IDCardImage != null)
                formData.Add(new StreamContent(IDCardImage.OpenReadStream()), "IDCardImage", IDCardImage.FileName);

            if (DriverLicenseImage != null)
                formData.Add(new StreamContent(DriverLicenseImage.OpenReadStream()), "DriverLicenseImage", DriverLicenseImage.FileName);

            var response = await _api.PutMultipartAsync($"api/Profile/{userId}", formData, token);

            if (response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Message"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Cập nhật hồ sơ thất bại!";
            return View(model);
        }

        // 🟢 Lịch sử thuê xe
        public async Task<IActionResult> History()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type.EndsWith("/nameidentifier"))?.Value;

            var response = await _api.GetAsync($"api/Profile/{userId}/rentals", token);
            var rentals = JsonConvert.DeserializeObject<List<RentalHistoryViewModel>>(response);
            return View(rentals);
        }
    }
}
