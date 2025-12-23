using CarRentalMVC.Helps;
using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace CarRentalMVC.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _context;
        private const string apiBaseUrl = "http://localhost:5000";

        public RentalsController(ApiService api, IHttpContextAccessor context)
        {
            _api = api;
            _context = context;
        }

        private int GetUserId()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return 0;

            return JwtHelper.GetUserIdFromToken(token) ?? 0;
        }

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

        // ==================== USER: TẠO ĐƠN THUÊ ====================
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // Lấy thông tin xe
            var vehicleJson = await _api.GetAsync($"api/Vehicles/{id}");
            var vehicle = JsonConvert.DeserializeObject<VehicleViewModel>(vehicleJson);

            // Lấy danh sách chi nhánh
            var branchesJson = await _api.GetAsync("api/Branches");
            var branches = JsonConvert.DeserializeObject<List<BranchViewModel>>(branchesJson);

            var model = new CreateRentalViewModel
            {
                VehicleID = vehicle.VehicleID,
                VehicleName = vehicle.VehicleName,
                VehicleImage = vehicle.VehicleImage,
                DailyRate = vehicle.PricePerDay,
                Seats = vehicle.Seats,
                BrandName = vehicle.BrandName,
                TypeName = vehicle.TypeName,
                RentalDate = DateTime.Now.AddDays(1),
                ReturnDate = DateTime.Now.AddDays(2)
            };

            ViewBag.Branches = branches;
            ViewBag.ApiBase = apiBaseUrl;

            return View(model);
        }

        /// <summary>
        /// Tạo đơn thuê xe
        /// Kiểm tra xe có sẵn sàng (Status = "Available") và không bị đặt trong khoảng thời gian này
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateRentalViewModel model)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            try
            {
                // Kiểm tra xe có sẵn sàng không (lấy lại thông tin xe từ API)
                var vehicleJson = await _api.GetAsync($"api/Vehicles/{model.VehicleID}");
                var vehicle = JsonConvert.DeserializeObject<VehicleViewModel>(vehicleJson);

                // Kiểm tra xe có sẵn sàng không
                if (vehicle == null)
                {
                    ViewBag.Error = "Xe không tồn tại!";
                    return await ReloadCreateView(model);
                }

                // Kiểm tra trạng thái xe - Nếu xe đã được đặt (Status != "Available") thì không cho phép đặt
                if (vehicle.Status != "Available")
                {
                    ViewBag.Error = $"Xe này đã được đặt. Trạng thái hiện tại: {vehicle.Status}. Vui lòng chọn xe khác.";
                    return await ReloadCreateView(model);
                }

                // Kiểm tra xe có đang hoạt động không
                if (!vehicle.IsActive)
                {
                    ViewBag.Error = "Xe này hiện không khả dụng. Vui lòng chọn xe khác.";
                    return await ReloadCreateView(model);
                }

                // Tạo đơn thuê
                var createRequest = new
                {
                    vehicleId = model.VehicleID,
                    rentalDate = model.RentalDate.ToString("yyyy-MM-dd"),
                    returnDate = model.ReturnDate.ToString("yyyy-MM-dd")
                };

                var response = await _api.PostAsync("api/Rentals/create", createRequest, token);
                var json = JObject.Parse(response);

                if (response.Contains("success", StringComparison.OrdinalIgnoreCase) ||
                    response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "Tạo đơn thuê thành công! Chờ admin duyệt.";
                    return RedirectToAction("MyRentals");
                }

                ViewBag.Error = json["error"]?.ToString() ?? "Tạo đơn thuê thất bại!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return await ReloadCreateView(model);
        }

        /// <summary>
        /// Helper method để reload dữ liệu cho Create view
        /// </summary>
        private async Task<IActionResult> ReloadCreateView(CreateRentalViewModel model)
        {
            var vehicleJson = await _api.GetAsync($"api/Vehicles/{model.VehicleID}");
            var vehicle = JsonConvert.DeserializeObject<VehicleViewModel>(vehicleJson);
            var branchesJson = await _api.GetAsync("api/Branches");
            var branches = JsonConvert.DeserializeObject<List<BranchViewModel>>(branchesJson);

            if (vehicle != null)
            {
                model.VehicleName = vehicle.VehicleName;
                model.VehicleImage = vehicle.VehicleImage;
                model.DailyRate = vehicle.PricePerDay;
                model.Seats = vehicle.Seats;
                model.BrandName = vehicle.BrandName;
                model.TypeName = vehicle.TypeName;
            }

            ViewBag.Branches = branches;
            ViewBag.ApiBase = apiBaseUrl;
            return View(model);
        }

        // ==================== USER: DANH SÁCH ĐƠN CỦA TÔI ====================
        [HttpGet]
        public async Task<IActionResult> MyRentals()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            try
            {
                var response = await _api.GetAsync("api/Rentals/my-rentals", token);
                var rentals = JsonConvert.DeserializeObject<List<RentalViewModel>>(response) ?? new List<RentalViewModel>();

                ViewBag.ApiBase = apiBaseUrl;
                return View(rentals);
            }
            catch
            {
                ViewBag.Error = "Không thể tải danh sách đơn thuê!";
                return View(new List<RentalViewModel>());
            }
        }

        // ==================== USER: CHI TIẾT ĐƠN THUÊ ====================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            try
            {
                var response = await _api.GetAsync($"api/Rentals/{id}", token);
                var rental = JsonConvert.DeserializeObject<RentalViewModel>(response);

                if (rental == null)
                    return NotFound();

                ViewBag.ApiBase = apiBaseUrl;
                return View(rental);
            }
            catch
            {
                return NotFound("Không tìm thấy đơn thuê!");
            }
        }

        // ==================== USER: HỦY ĐƠN THUÊ ====================
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            try
            {
                var response = await _api.PostAsync($"api/Rentals/{id}/cancel", new { }, token);

                if (response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "Hủy đơn thuê thành công!";
                    return RedirectToAction("MyRentals");
                }

                TempData["Error"] = "Hủy đơn thất bại! (Chỉ có thể hủy đơn ở trạng thái Pending)";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id });
        }

        // ==================== USER: TÍNH GIÁ REALTIME ====================
        [HttpPost]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculatePriceViewModel model)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            try
            {
                var request = new
                {
                    vehicleId = model.VehicleID,
                    rentalDate = model.RentalDate.ToString("yyyy-MM-dd"),
                    returnDate = model.ReturnDate.ToString("yyyy-MM-dd")
                };

                var response = await _api.PostAsync("api/Rentals/calculate-price", request, token);
                var result = JsonConvert.DeserializeObject<PriceResponseViewModel>(response);

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ==================== ADMIN: DANH SÁCH TẤT CẢ ĐƠN ====================
        [HttpGet]
        public async Task<IActionResult> AdminList(string? status = null)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token) || !IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                var endpoint = "api/Rentals/admin/all";
                if (!string.IsNullOrEmpty(status))
                    endpoint += $"?status={status}";

                var response = await _api.GetAsync(endpoint, token);
                var rentals = JsonConvert.DeserializeObject<List<RentalViewModel>>(response) ?? new List<RentalViewModel>();

                ViewBag.CurrentStatus = status;
                ViewBag.ApiBase = apiBaseUrl;

                return View(rentals);
            }
            catch
            {
                ViewBag.Error = "Không thể tải danh sách!";
                return View(new List<RentalViewModel>());
            }
        }

        // ==================== ADMIN: DUYỆT ĐƠN ====================
        [HttpPost]
        public async Task<IActionResult> AdminApprove(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token) || !IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                var response = await _api.PostAsync($"api/Rentals/admin/{id}/approve", new { }, token);

                if (response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "Duyệt đơn thành công!";
                    return RedirectToAction("AdminList");
                }

                TempData["Error"] = "Duyệt đơn thất bại!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("AdminList");
        }

        // ==================== ADMIN: TỪ CHỐI ĐƠN ====================
        [HttpPost]
        public async Task<IActionResult> AdminReject(int id, string? reason)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token) || !IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                var request = new { reason = reason ?? "" };
                var response = await _api.PostAsync($"api/Rentals/admin/{id}/reject", request, token);

                if (response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "Từ chối đơn thành công!";
                    return RedirectToAction("AdminList");
                }

                TempData["Error"] = "Từ chối đơn thất bại!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("AdminList");
        }

        // ==================== ADMIN: XÁC NHẬN TRẢ XE ====================
        /// <summary>
        /// Admin xác nhận đã trả xe
        /// Cập nhật trạng thái đơn thành "Completed" và trạng thái xe về "Available"
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AdminConfirmReturn(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token) || !IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                // Gọi API để xác nhận trả xe
                var response = await _api.PostAsync($"api/Rentals/admin/{id}/confirm-return", new { }, token);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                if (response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Message"] = "Xác nhận trả xe thành công. Trạng thái xe đã được cập nhật về 'Available'.";
                    return RedirectToAction("AdminList");
                }

                TempData["Error"] = "Xác nhận trả xe thất bại!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xác nhận trả xe: {ex.Message}";
            }

            return RedirectToAction("AdminList");
        }

        // ==================== ADMIN: THỐNG KÊ ====================
        [HttpGet]
        public async Task<IActionResult> AdminStatistics()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            if (string.IsNullOrEmpty(token) || !IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                var response = await _api.GetAsync("api/Rentals/admin/statistics", token);
                var stats = JsonConvert.DeserializeObject<RentalStatisticsViewModel>(response);

                ViewBag.ApiBase = apiBaseUrl;
                return View(stats);
            }
            catch
            {
                ViewBag.Error = "Không thể tải thống kê!";
                return View(new RentalStatisticsViewModel());
            }
        }
    }
}
 