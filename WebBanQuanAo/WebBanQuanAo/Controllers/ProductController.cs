using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;
using WebBanQuanAo.Serveice;

namespace WebBanQuanAo.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReviewService _reviewService;

        public ProductController(ApplicationDbContext context, IReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;
        }

        // Kiểm tra quyền truy cập (chỉ cho Admin)
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        // Danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // Lấy đánh giá và rating trung bình
            var reviews = await _reviewService.GetProductReviews(id.Value);
            var averageRating = await _reviewService.GetAverageRating(id.Value);
            var reviewCount = await _reviewService.GetReviewCount(id.Value);
            
            var userId = HttpContext.Session.GetInt32("UserId");
            var hasReviewed = userId.HasValue ? await _reviewService.HasUserReviewed(id.Value, userId.Value) : false;

            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.ReviewCount = reviewCount;
            ViewBag.HasReviewed = hasReviewed;

            return View(product);
        }
        // Hiển thị form thêm sản phẩm
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            // Make sure categories are loaded and not null
            var categories = _context.Categories.ToList();
            if (categories == null || !categories.Any())
            {
                // Handle the case where no categories exist
                TempData["Error"] = "Không tìm thấy danh mục sản phẩm nào.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = categories;
            return View();
        }

        // Xử lý thêm sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, string ListImg, List<string> AvailableSizes, List<string> AvailableColors)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra nếu CategoryId không hợp lệ
            if (product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Vui lòng chọn một danh mục.");
            }

            if (!ModelState.IsValid)
            {
                // Đảm bảo rằng Categories được load lại khi form không hợp lệ
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }

            // Chuyển đổi ListImg thành một danh sách các URL
            if (!string.IsNullOrEmpty(ListImg))
            {
                product.ListImg = ListImg.Split(',').Select(img => img.Trim()).ToList();
            }
            else
            {
                product.ListImg = new List<string>();  // Nếu không có hình ảnh phụ, gán danh sách trống
            }
            
            // Gán Size và Color
            product.AvailableSizes = AvailableSizes ?? new List<string>();
            product.AvailableColors = AvailableColors ?? new List<string>();

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }
        }



        // Hiển thị form sửa sản phẩm
        // Hiển thị form sửa sản phẩm
        // Hiển thị form sửa sản phẩm
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Make sure to load categories for the dropdown
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(product);
        }

        // Xử lý sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            if (id != product.ProductId) return NotFound();

            // Xử lý ListImg (tách các URL từ chuỗi nhập vào và lưu vào List<string>)
            if (!string.IsNullOrEmpty(Request.Form["ListImg"]))
            {
                product.ListImg = Request.Form["ListImg"].ToString()
                    .Split(',')
                    .Select(url => url.Trim())
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }


        // Hiển thị xác nhận xóa sản phẩm
        // Hiển thị xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // Xử lý xóa sản phẩm
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
            }

            return RedirectToAction(nameof(Index)); // Trở về danh sách sản phẩm
        }


    }
}
