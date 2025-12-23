using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CarRentalMVC.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApiService _api;

        public VehiclesController(ApiService api)
        {
            _api = api;
        }

        // ✅ Danh sách xe
        public async Task<IActionResult> Index()
        {
            var json = await _api.GetAsync("api/Vehicles");
            var vehicles = JsonConvert.DeserializeObject<List<VehicleViewModel>>(json) ?? new List<VehicleViewModel>();
            return View(vehicles);
        }

        // ✅ Trang thêm mới
        public async Task<IActionResult> Create()
        {
            var model = new VehicleViewModel();

            var brandsJson = await _api.GetAsync("api/Vehicles/brands");
            var typesJson = await _api.GetAsync("api/Vehicles/types");
            var branchesJson = await _api.GetAsync("api/Branches");

            model.Brands = JsonConvert.DeserializeObject<List<BrandViewModel>>(brandsJson);
            model.Types = JsonConvert.DeserializeObject<List<TypeViewModel>>(typesJson);
            model.Branches = JsonConvert.DeserializeObject<List<BranchViewModel>>(branchesJson);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(VehicleViewModel model)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(model.VehicleName), "VehicleName");
            form.Add(new StringContent(model.BrandID.ToString()), "BrandID");
            form.Add(new StringContent(model.TypeID.ToString()), "TypeID");
            form.Add(new StringContent(model.BranchID.ToString()), "BranchID");
            form.Add(new StringContent(model.PricePerDay.ToString()), "PricePerDay");
            form.Add(new StringContent(model.Seats.ToString()), "Seats");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.Status ?? "Available"), "Status");
            form.Add(new StringContent(model.IsActive.ToString()), "IsActive");

            if (model.ImageFile != null)
            {
                var stream = model.ImageFile.OpenReadStream();
                form.Add(new StreamContent(stream)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType) }
                }, "ImageFile", model.ImageFile.FileName);
            }

            await _api.PostMultipartAsync("api/Vehicles", form);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Sửa xe
        public async Task<IActionResult> Edit(int id)
        {
            var json = await _api.GetAsync($"api/Vehicles/{id}");
            var model = JsonConvert.DeserializeObject<VehicleViewModel>(json);

            var brandsJson = await _api.GetAsync("api/Vehicles/brands");
            var typesJson = await _api.GetAsync("api/Vehicles/types");
            var branchesJson = await _api.GetAsync("api/Branches");

            model.Brands = JsonConvert.DeserializeObject<List<BrandViewModel>>(brandsJson);
            model.Types = JsonConvert.DeserializeObject<List<TypeViewModel>>(typesJson);
            model.Branches = JsonConvert.DeserializeObject<List<BranchViewModel>>(branchesJson);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VehicleViewModel model)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(model.VehicleName), "VehicleName");
            form.Add(new StringContent(model.BrandID.ToString()), "BrandID");
            form.Add(new StringContent(model.TypeID.ToString()), "TypeID");
            form.Add(new StringContent(model.BranchID.ToString()), "BranchID");
            form.Add(new StringContent(model.PricePerDay.ToString()), "PricePerDay");
            form.Add(new StringContent(model.Seats.ToString()), "Seats");
            form.Add(new StringContent(model.Description ?? ""), "Description");
            form.Add(new StringContent(model.Status ?? "Available"), "Status");
            form.Add(new StringContent(model.IsActive.ToString()), "IsActive");

            if (model.ImageFile != null)
            {
                var stream = model.ImageFile.OpenReadStream();
                form.Add(new StreamContent(stream)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType) }
                }, "ImageFile", model.ImageFile.FileName);
            }

            await _api.PutMultipartAsync($"api/Vehicles/{id}", form);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Xem chi tiết xe
        public async Task<IActionResult> Details(int id)
        {
            var json = await _api.GetAsync($"api/Vehicles/{id}");
            var model = JsonConvert.DeserializeObject<VehicleViewModel>(json);
            return View(model);
        }

        // ✅ Xóa xe
        public async Task<IActionResult> Delete(int id)
        {
            await _api.DeleteAsync($"api/Vehicles/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
