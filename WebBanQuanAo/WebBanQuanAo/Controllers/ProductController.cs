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

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .ToListAsync();
            return View(products);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // Lấy tất cả dữ liệu review trong một lần gọi
            var userId = HttpContext.Session.GetInt32("UserId");
            var reviewData = await _reviewService.GetProductReviewData(id.Value, userId);

            ViewBag.Reviews = reviewData.Reviews;
            ViewBag.AverageRating = reviewData.AverageRating;
            ViewBag.ReviewCount = reviewData.ReviewCount;
            ViewBag.HasReviewed = reviewData.HasUserReviewed;

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
        public async Task<IActionResult> Create(Product product, string imageUrls, List<string> sizes, List<string> colors)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            if (product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Vui lòng chọn một danh mục.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(product);
            }

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Thêm hình ảnh
                if (!string.IsNullOrEmpty(imageUrls))
                {
                    var urls = imageUrls.Split(',').Select(url => url.Trim()).Where(url => !string.IsNullOrEmpty(url));
                    var images = urls.Select((url, index) => new ImageProduct
                    {
                        ProductId = product.ProductId,
                        ImageUrl = url,
                        IsMain = index == 0,
                        DisplayOrder = index + 1
                    }).ToList();
                    _context.ImageProducts.AddRange(images);
                }

                // Thêm sizes
                if (sizes != null && sizes.Any())
                {
                    var sizeProducts = sizes.Select(size => new SizeProduct
                    {
                        ProductId = product.ProductId,
                        SizeName = size,
                        Stock = product.Stock / sizes.Count
                    }).ToList();
                    _context.SizeProducts.AddRange(sizeProducts);
                }

                // Thêm colors
                if (colors != null && colors.Any())
                {
                    var colorProducts = colors.Select(color => new ColorProduct
                    {
                        ProductId = product.ProductId,
                        ColorName = color,
                        ColorCode = GetColorCode(color)
                    }).ToList();
                    _context.ColorProducts.AddRange(colorProducts);
                }

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

        private string GetColorCode(string colorName)
        {
            return colorName.ToLower() switch
            {
                "đen" or "black" => "#000000",
                "trắng" or "white" => "#FFFFFF",
                "xanh" or "blue" => "#0066CC",
                "đỏ" or "red" => "#FF0000",
                "vàng" or "yellow" => "#FFFF00",
                "xanh lá" or "green" => "#00FF00",
                _ => "#808080"
            };
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

            var product = await _context.Products
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // Xử lý sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, string imageUrls, List<string> sizes, List<string> colors)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);

                    // Xóa dữ liệu cũ
                    var existingImages = _context.ImageProducts.Where(i => i.ProductId == id);
                    var existingSizes = _context.SizeProducts.Where(s => s.ProductId == id);
                    var existingColors = _context.ColorProducts.Where(c => c.ProductId == id);
                    
                    _context.ImageProducts.RemoveRange(existingImages);
                    _context.SizeProducts.RemoveRange(existingSizes);
                    _context.ColorProducts.RemoveRange(existingColors);

                    // Thêm dữ liệu mới
                    if (!string.IsNullOrEmpty(imageUrls))
                    {
                        var urls = imageUrls.Split(',').Select(url => url.Trim()).Where(url => !string.IsNullOrEmpty(url));
                        var images = urls.Select((url, index) => new ImageProduct
                        {
                            ProductId = id,
                            ImageUrl = url,
                            IsMain = index == 0,
                            DisplayOrder = index + 1
                        }).ToList();
                        _context.ImageProducts.AddRange(images);
                    }

                    if (sizes != null && sizes.Any())
                    {
                        var sizeProducts = sizes.Select(size => new SizeProduct
                        {
                            ProductId = id,
                            SizeName = size,
                            Stock = product.Stock / sizes.Count
                        }).ToList();
                        _context.SizeProducts.AddRange(sizeProducts);
                    }

                    if (colors != null && colors.Any())
                    {
                        var colorProducts = colors.Select(color => new ColorProduct
                        {
                            ProductId = id,
                            ColorName = color,
                            ColorCode = GetColorCode(color)
                        }).ToList();
                        _context.ColorProducts.AddRange(colorProducts);
                    }

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
