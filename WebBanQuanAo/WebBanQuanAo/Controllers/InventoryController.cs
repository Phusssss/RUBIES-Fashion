using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Attributes;
using WebBanQuanAo.Models;
using WebBanQuanAo.Serveice;

namespace WebBanQuanAo.Controllers
{
    [AuthorizeRole("Admin")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;

        public InventoryController(IInventoryService inventoryService, ApplicationDbContext context)
        {
            _inventoryService = inventoryService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lowStockProducts = await _inventoryService.GetLowStockProducts();
            return View(lowStockProducts);
        }

        public async Task<IActionResult> History(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            ViewBag.Product = product;
            var history = await _inventoryService.GetInventoryHistory(productId);
            return View(history);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStock(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            ViewBag.Product = product;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int quantity, string type, string reason)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var success = await _inventoryService.UpdateStock(productId, quantity, type, reason, userId);

            if (success)
            {
                TempData["Success"] = "Cập nhật kho thành công!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Cập nhật kho thất bại!";
            return RedirectToAction("UpdateStock", new { productId });
        }
    }
}