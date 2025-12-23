using Microsoft.AspNetCore.Mvc;

namespace CarRentalMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
