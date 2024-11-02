using Microsoft.AspNetCore.Mvc;

namespace SetorDeCompras.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
