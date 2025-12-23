using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;


namespace CarRentalMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _context;

        public AccountController(ApiService api, IHttpContextAccessor context)
        {
            _api = api;
            _context = context;
        }

        // === REGISTER ===
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var response = await _api.PostAsync("api/Auth/register", model);
            if (response.Contains("success", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login");

            ViewBag.Error = "Đăng ký thất bại!";
            return View(model);
        }

        // === LOGIN ===
        public IActionResult Login() => View();

        // === LOGIN ===
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var response = await _api.PostAsync("api/Auth/login", model);
            var json = JObject.Parse(response);
            var token = json["token"]?.ToString();

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Sai email hoặc mật khẩu!";
                return View(model);
            }

            // ✅ Lưu Token vào Session
            HttpContext.Session.SetString("Token", token);

            // ✅ Giải mã lấy UserId & Role
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "nameid"
                                                    || c.Type.EndsWith("nameidentifier"))?.Value;
            var role = jwt.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value;

            if (!string.IsNullOrEmpty(userId))
                HttpContext.Session.SetString("UserId", userId);

            if (role == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }


        // === LOGOUT ===
        public IActionResult Logout()
        {
            _context.HttpContext!.Session.Clear();
            return RedirectToAction("Login");
        }

        // === CHANGE PASSWORD ===
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var response = await _api.PostAsync("api/Auth/change-password", model, token);

            if (response.Contains("Đổi mật khẩu thành công", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Message"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = response;
            return View(model);
        }

    }
}
