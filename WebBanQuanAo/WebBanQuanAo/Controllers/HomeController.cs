using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;
using WebBanQuanAo.Serveice;

namespace WebBanQuanAo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReviewService _reviewService;

        public HomeController(ApplicationDbContext context, IReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(bool? isNew, bool? isTrend)
        {
            var categories = await _context.Categories.ToListAsync();

            if (categories == null)
            {
                return View("Error");
            }

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .AsQueryable();

            // Filter by IsNew
            if (isNew.HasValue)
            {
                products = products.Where(p => p.IsNew == isNew.Value);
            }

            // Filter by IsTrend
            if (isTrend.HasValue)
            {
                products = products.Where(p => p.IsTrend == isTrend.Value);
            }

            var productList = await products.ToListAsync();

            if (productList == null)
            {
                return View("Error");
            }

            // Lấy rating trung bình cho từng sản phẩm trong một lần truy vấn
            var productIds = productList.Select(p => p.ProductId).ToList();
            var productRatings = await GetProductRatings(productIds);

            ViewBag.Categories = categories;
            ViewBag.ProductRatings = productRatings;
            return View(productList);
        }


        // Hiển thị sản phẩm theo danh mục
        public async Task<IActionResult> ProductsByCategory(int? categoryId)
        {
            if (categoryId == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .ToListAsync();

            // Kiểm tra nếu sản phẩm là null
            if (products == null)
            {
                return NotFound();
            }

            // Lấy rating trung bình cho từng sản phẩm trong một lần truy vấn
            var productIds = products.Select(p => p.ProductId).ToList();
            var productRatings = await GetProductRatings(productIds);
            ViewBag.ProductRatings = productRatings;

            return View(products);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy tất cả dữ liệu review trong một lần gọi
            var userId = HttpContext.Session.GetInt32("UserId");
            var reviewData = await _reviewService.GetProductReviewData(id.Value, userId);

            ViewBag.Reviews = reviewData.Reviews;
            ViewBag.AverageRating = reviewData.AverageRating;
            ViewBag.ReviewCount = reviewData.ReviewCount;
            ViewBag.HasReviewed = reviewData.HasUserReviewed;

            return View(product);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1, string selectedSize = null, string selectedColor = null)
        {
            try
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
                
                var mainImage = _context.ImageProducts
                    .Where(i => i.ProductId == productId && i.IsMain)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault();

                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = mainImage,
                    Quantity = quantity,
                    SelectedSize = selectedSize,
                    SelectedColor = selectedColor,
                    Product = product
                };
                
                // Tìm item có cùng sản phẩm, size, màu
                var existingItem = cart.FirstOrDefault(c => c.CartKey == cartItem.CartKey);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(cartItem);
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);
                
                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        private async Task<Dictionary<int, double>> GetProductRatings(List<int> productIds)
        {
            var ratings = await _context.Reviews
                .Where(r => productIds.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .Select(g => new { ProductId = g.Key, AverageRating = g.Average(r => r.Rating) })
                .ToListAsync();

            return ratings.ToDictionary(r => r.ProductId, r => r.AverageRating);
        }
    }
}
