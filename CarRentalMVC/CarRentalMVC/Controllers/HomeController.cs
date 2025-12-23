using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CarRentalMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApiService _api;

        public HomeController(ApiService api)
        {
            _api = api;
        }

        /// <summary>
        /// Trang chủ: Danh sách xe có phân trang
        /// Hiển thị trạng thái xe dựa trên Status: Available (Sẵn sàng), Reserved/Rented (Đã đặt xe)
        /// </summary>
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            try
            {
                // Lấy danh sách xe từ API
                var json = await _api.GetAsync("api/Vehicles");
                var vehicles = JsonConvert.DeserializeObject<List<VehicleViewModel>>(json) ?? new List<VehicleViewModel>();

                // Phân trang
                var totalItems = vehicles.Count;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagedVehicles = vehicles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;

                return View(pagedVehicles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi tải danh sách xe: {ex.Message}";
                return View(new List<VehicleViewModel>());
            }
        }

        // ✅ Xem chi tiết xe
        public async Task<IActionResult> Details(int id)
        {
            var json = await _api.GetAsync($"api/Vehicles/{id}");
            if (string.IsNullOrEmpty(json))
                return NotFound();

            var vehicle = JsonConvert.DeserializeObject<VehicleViewModel>(json);
            return View(vehicle);
        }

        public async Task<IActionResult> test(int id)
        {
            var json = await _api.GetAsync($"api/Vehicles/{id}");
            if (string.IsNullOrEmpty(json))
                return NotFound();

            var vehicle = JsonConvert.DeserializeObject<VehicleViewModel>(json);
            return View(vehicle);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // ✅ Thêm vào cuối HomeController
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var json = await _api.GetAsync("api/Vehicles");
            var vehicles = JsonConvert.DeserializeObject<List<VehicleViewModel>>(json) ?? new List<VehicleViewModel>();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                vehicles = vehicles
                    .Where(v => v.VehicleName.ToLower().Contains(keyword)
                             || v.BrandName.ToLower().Contains(keyword)
                             || v.TypeName.ToLower().Contains(keyword))
                    .ToList();
            }

            return PartialView("_VehicleListPartial", vehicles);
        }

    }
}
