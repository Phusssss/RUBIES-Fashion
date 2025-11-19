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

            var products = _context.Products.Include(p => p.Category).AsQueryable();

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

            // Lấy rating trung bình cho từng sản phẩm
            var productRatings = new Dictionary<int, double>();
            foreach (var product in productList)
            {
                var avgRating = await _reviewService.GetAverageRating(product.ProductId);
                productRatings[product.ProductId] = avgRating;
            }

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
                .ToListAsync();

            // Kiểm tra nếu sản phẩm là null
            if (products == null)
            {
                return NotFound();
            }

            // Lấy rating trung bình cho từng sản phẩm
            var productRatings = new Dictionary<int, double>();
            foreach (var product in products)
            {
                var avgRating = await _reviewService.GetAverageRating(product.ProductId);
                productRatings[product.ProductId] = avgRating;
            }
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
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

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

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1, string selectedSize = null, string selectedColor = null)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            
            var cartItem = new CartItem
            {
                Product = product,
                Quantity = quantity,
                SelectedSize = selectedSize,
                SelectedColor = selectedColor
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
            
            if (Request.Headers["Content-Type"].ToString().Contains("application/json"))
            {
                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công" });
            }
            
            return RedirectToAction("Index");
        }
    }
}
