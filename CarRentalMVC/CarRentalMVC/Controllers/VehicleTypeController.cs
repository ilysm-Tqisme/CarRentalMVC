using CarRentalMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalMVC.Controllers
{
    public class VehicleTypeController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _apiUrl = "http://localhost:5000/api/VehicleTypes";

        public VehicleTypeController()
        {
            _client = new HttpClient();
        }

        // ✅ Danh sách loại xe
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync(_apiUrl);
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<VehicleTypeViewModel>>(json) ?? new();

            return View(list);
        }

        // ✅ GET: Thêm loại xe
        public IActionResult Create()
        {
            return View();
        }

        // ✅ POST: Thêm loại xe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleTypeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = await response.Content.ReadAsStringAsync();
            return View(model);
        }

        // ✅ GET: Sửa loại xe
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _client.GetAsync($"{_apiUrl}/{id}");
            if (!res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var json = await res.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<VehicleTypeViewModel>(json);
            return View(model);
        }

        // ✅ POST: Sửa loại xe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleTypeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var res = await _client.PutAsync($"{_apiUrl}/{id}", content);
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = await res.Content.ReadAsStringAsync();
            return View(model);
        }

        // ✅ Xóa loại xe
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _client.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
