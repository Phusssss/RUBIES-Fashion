using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;
using WebBanQuanAo.Serveice;

namespace WebBanQuanAo.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPromotionService _promotionService;

        public PromotionController(ApplicationDbContext context, IPromotionService promotionService)
        {
            _context = context;
            _promotionService = promotionService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // Quản lý Coupon
        public async Task<IActionResult> Coupons()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            var coupons = await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View(coupons);
        }

        public IActionResult CreateCoupon()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(Coupon coupon)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            if (ModelState.IsValid)
            {
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo mã giảm giá thành công!";
                return RedirectToAction(nameof(Coupons));
            }
            return View(coupon);
        }

        // Quản lý Flash Sale
        public async Task<IActionResult> FlashSales()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            var flashSales = await _context.FlashSales
                .Include(f => f.Product)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return View(flashSales);
        }

        public async Task<IActionResult> CreateFlashSale()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            ViewBag.Products = await _context.Products.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashSale(FlashSale flashSale)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            if (ModelState.IsValid)
            {
                flashSale.DiscountPercentage = (int)((flashSale.OriginalPrice - flashSale.SalePrice) / flashSale.OriginalPrice * 100);
                _context.FlashSales.Add(flashSale);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo flash sale thành công!";
                return RedirectToAction(nameof(FlashSales));
            }
            
            ViewBag.Products = await _context.Products.ToListAsync();
            return View(flashSale);
        }

        // Quản lý khuyến mãi theo danh mục
        public async Task<IActionResult> CategoryPromotions()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            var promotions = await _context.CategoryPromotions
                .Include(cp => cp.Category)
                .OrderByDescending(cp => cp.CreatedAt)
                .ToListAsync();
            return View(promotions);
        }

        public async Task<IActionResult> CreateCategoryPromotion()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryPromotion(CategoryPromotion promotion)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            
            if (ModelState.IsValid)
            {
                _context.CategoryPromotions.Add(promotion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo khuyến mãi danh mục thành công!";
                return RedirectToAction(nameof(CategoryPromotions));
            }
            
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(promotion);
        }

        // API để validate coupon
        [HttpPost]
        public async Task<IActionResult> ValidateCoupon(string code, decimal orderAmount)
        {
            var coupon = await _promotionService.ValidateCoupon(code, orderAmount);
            if (coupon != null)
            {
                var discount = await _promotionService.ApplyCoupon(code, orderAmount);
                return Json(new { success = true, discount = discount, message = "Mã giảm giá hợp lệ!" });
            }
            return Json(new { success = false, message = "Mã giảm giá không hợp lệ!" });
        }

        // Hiển thị flash sales cho khách hàng
        public async Task<IActionResult> FlashSaleProducts()
        {
            var flashSales = await _promotionService.GetActiveFlashSales();
            return View(flashSales);
        }
    }
}