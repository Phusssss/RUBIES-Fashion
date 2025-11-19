using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Attributes;

namespace WebBanQuanAo.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
