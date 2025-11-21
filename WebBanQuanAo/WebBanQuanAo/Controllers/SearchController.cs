using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, 
            string size, string color, string sortBy = "name")
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .AsQueryable();

            // Tìm kiếm full-text
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || 
                                        p.Description.Contains(searchTerm) ||
                                        p.Category.Name.Contains(searchTerm));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            // Lọc theo size
            if (!string.IsNullOrEmpty(size))
            {
                query = query.Where(p => p.Sizes.Any(s => s.SizeName == size));
            }

            // Lọc theo màu
            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(p => p.Colors.Any(c => c.ColorName == color));
            }

            // Sắp xếp
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.ProductId),
                _ => query.OrderBy(p => p.Name)
            };

            var products = await query.ToListAsync();

            // Gợi ý sản phẩm liên quan
            var suggestions = new List<Product>();
            if (!string.IsNullOrEmpty(searchTerm) && products.Count < 5)
            {
                suggestions = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => !products.Contains(p) && 
                               (p.Category.Name.Contains(searchTerm) || p.Name.Contains(searchTerm)))
                    .Take(3)
                    .ToListAsync();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Size = size;
            ViewBag.Color = color;
            ViewBag.SortBy = sortBy;
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Suggestions = suggestions;

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Autocomplete(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new List<string>());

            var suggestions = await _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => p.Name)
                .Distinct()
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }
    }
}