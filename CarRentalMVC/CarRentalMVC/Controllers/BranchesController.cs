using CarRentalMVC.Models;
using CarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarRentalMVC.Controllers
{
    public class BranchesController : Controller
    {
        private readonly ApiService _api;
        private readonly IHttpContextAccessor _context;
        private readonly string apiBaseUrl = "http://localhost:5000";

        public BranchesController(ApiService api, IHttpContextAccessor context)
        {
            _api = api;
            _context = context;
        }

        // GET: Danh sách chi nhánh
        public async Task<IActionResult> Index()
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            var response = await _api.GetAsync("api/Branches", token);
            var branches = JsonConvert.DeserializeObject<List<BranchViewModel>>(response);

            ViewBag.ApiBase = apiBaseUrl;
            return View(branches);
        }

        // GET: Thêm chi nhánh
        public IActionResult Create()
        {
            ViewBag.Provinces = GetVietnamProvinces();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchCreateModel model)
        {
            ViewBag.Provinces = GetVietnamProvinces();

            // Kiểm tra hợp lệ
            if (!ModelState.IsValid)
                return View(model);

            // Gọi API để kiểm tra trùng tên và SDT
            var token = _context.HttpContext!.Session.GetString("Token");
            var checkResponse = await _api.GetAsync("api/Branches/check-duplicate?name=" + model.BranchName + "&phone=" + model.Phone, token);
            if (checkResponse.Contains("true"))
            {
                ViewBag.Error = "Tên chi nhánh hoặc số điện thoại đã tồn tại!";
                return View(model);
            }

            // Upload ảnh
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.BranchName), "BranchName");
            content.Add(new StringContent(model.Address), "Address");
            if (!string.IsNullOrEmpty(model.Phone))
                content.Add(new StringContent(model.Phone), "Phone");
            if (!string.IsNullOrEmpty(model.Description))
                content.Add(new StringContent(model.Description), "Description");

            if (model.ImageFile != null)
            {
                var fileStream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                content.Add(fileContent, "ImageFile", model.ImageFile.FileName);
            }

            await _api.PostMultipartAsync("api/Branches", content, token);
            return RedirectToAction("Index");
        }

        // GET: Sửa chi nhánh
        public async Task<IActionResult> Edit(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            var response = await _api.GetAsync($"api/Branches/{id}", token);
            var branch = JsonConvert.DeserializeObject<BranchViewModel>(response);

            ViewBag.ApiBase = apiBaseUrl;
            ViewBag.Provinces = GetVietnamProvinces();

            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BranchViewModel model)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            ViewBag.Provinces = GetVietnamProvinces();

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.BranchID.ToString()), "BranchID");
            content.Add(new StringContent(model.BranchName), "BranchName");
            content.Add(new StringContent(model.Address), "Address");
            content.Add(new StringContent(model.Phone ?? ""), "Phone");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.BranchImage ?? ""), "BranchImage");

            if (model.ImageFile != null)
            {
                var fileStream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                content.Add(fileContent, "ImageFile", model.ImageFile.FileName);
            }

            await _api.PutMultipartAsync($"api/Branches/{model.BranchID}", content, token);
            return RedirectToAction("Index");
        }

        // Xóa
        public async Task<IActionResult> Delete(int id)
        {
            var token = _context.HttpContext!.Session.GetString("Token");
            await _api.DeleteAsync($"api/Branches/{id}", token);
            return RedirectToAction("Index");
        }

        // Danh sách tỉnh Việt Nam
        private List<string> GetVietnamProvinces()
        {
            return new List<string> {
                "Hà Nội", "TP. Hồ Chí Minh", "Đà Nẵng", "Hải Phòng", "Cần Thơ",
                "An Giang", "Bà Rịa - Vũng Tàu", "Bắc Giang", "Bắc Ninh", "Bến Tre",
                "Bình Dương", "Bình Định", "Bình Phước", "Bình Thuận", "Cà Mau",
                "Đắk Lắk", "Đắk Nông", "Điện Biên", "Đồng Nai", "Đồng Tháp",
                "Gia Lai", "Hà Giang", "Hà Nam", "Hà Tĩnh", "Hải Dương",
                "Hậu Giang", "Hoà Bình", "Khánh Hoà", "Kiên Giang", "Kon Tum",
                "Lai Châu", "Lâm Đồng", "Lạng Sơn", "Long An", "Nam Định",
                "Nghệ An", "Ninh Bình", "Ninh Thuận", "Phú Thọ", "Phú Yên",
                "Quảng Bình", "Quảng Nam", "Quảng Ngãi", "Quảng Ninh", "Quảng Trị",
                "Sóc Trăng", "Sơn La", "Tây Ninh", "Thái Bình", "Thái Nguyên",
                "Thanh Hoá", "Thừa Thiên Huế", "Tiền Giang", "Trà Vinh", "Tuyên Quang",
                "Vĩnh Long", "Vĩnh Phúc", "Yên Bái"
            };
        }
    }
}
