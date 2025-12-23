using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalMVC.Controllers
{
    public class VehicleBrandController : Controller
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _context;

        public VehicleBrandController(ApiService api, IHttpContextAccessor context)
        {
            _api = api;
            _context = context;
        }

        // 🧩 Danh sách hãng xe
        public async Task<IActionResult> Index()
        {
            var response = await _api.GetAsync("api/VehicleBrands");
            var brands = JsonConvert.DeserializeObject<List<VehicleBrandViewModel>>(response) ?? new();
            ViewBag.ApiBase = _api.BaseUrl; // để load ảnh
            return View(brands);
        }

        // 🧩 Thêm hãng xe
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(VehicleBrandViewModel model, IFormFile? logo)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.BrandName ?? ""), "BrandName");
            form.Add(new StringContent(model.Description ?? ""), "Description");

            if (logo != null)
            {
                var fileStream = new StreamContent(logo.OpenReadStream());
                form.Add(fileStream, "logo", logo.FileName);
            }

            var response = await _api.PostMultipartAsync("api/VehicleBrands", form);

            if (response.Contains("success", StringComparison.OrdinalIgnoreCase) ||
                response.Contains("Thêm hãng xe thành công", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            ViewBag.Error = "❌ Thêm thất bại!";
            return View(model);
        }

        // 🧩 Sửa hãng xe
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _api.GetAsync($"api/VehicleBrands/{id}");
            var brand = JsonConvert.DeserializeObject<VehicleBrandViewModel>(response);
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VehicleBrandViewModel model, IFormFile? logo)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(model.BrandName ?? ""), "BrandName");
            form.Add(new StringContent(model.Description ?? ""), "Description");

            if (logo != null)
            {
                var fileStream = new StreamContent(logo.OpenReadStream());
                form.Add(fileStream, "logo", logo.FileName);
            }

            var response = await _api.PutMultipartAsync($"api/VehicleBrands/{id}", form);

            if (response.Contains("thành công", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index");

            ViewBag.Error = "❌ Cập nhật thất bại!";
            return View(model);
        }

        // 🧩 Xóa hãng xe
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteAsync($"api/VehicleBrands/{id}");
            return RedirectToAction("Index");
        }
    }
}
