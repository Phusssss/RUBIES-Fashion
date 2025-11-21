using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class DebugController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DebugController> _logger;

        public DebugController(ApplicationDbContext context, ILogger<DebugController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> CheckProducts()
        {
            var products = await _context.Products
                .Include(p => p.Colors)
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .ToListAsync();
            
            var debugInfo = products.Select(p => new
            {
                p.ProductId,
                p.Name,
                NameLength = p.Name?.Length ?? 0,
                p.Description,
                DescriptionLength = p.Description?.Length ?? 0,
                Colors = p.Colors.Select(c => c.ColorName).ToArray(),
                Sizes = p.Sizes.Select(s => s.SizeName).ToArray(),
                Images = p.Images.Select(i => i.ImageUrl).ToArray(),
                HasColors = p.Colors.Any(),
                HasSizes = p.Sizes.Any(),
                HasImages = p.Images.Any()
            }).ToList();

            return Json(debugInfo);
        }

        public async Task<IActionResult> FixProductData(int productId = 0)
        {
            int fixedCount = 0;
            
            if (productId > 0)
            {
                // Sửa một sản phẩm cụ thể
                var product = await _context.Products
                    .Include(p => p.Colors)
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                    return NotFound();

                if (!product.Sizes.Any())
                {
                    var defaultSizes = new[] { "S", "M", "L", "XL" };
                    foreach (var size in defaultSizes)
                    {
                        _context.SizeProducts.Add(new SizeProduct
                        {
                            ProductId = productId,
                            SizeName = size,
                            Stock = product.Stock / defaultSizes.Length
                        });
                    }
                    fixedCount++;
                }
                
                if (!product.Colors.Any())
                {
                    var defaultColors = new[] { "Đen", "Trắng", "Xanh" };
                    foreach (var color in defaultColors)
                    {
                        _context.ColorProducts.Add(new ColorProduct
                        {
                            ProductId = productId,
                            ColorName = color,
                            ColorCode = GetColorCode(color)
                        });
                    }
                    fixedCount++;
                }
            }
            else
            {
                // Sửa tất cả sản phẩm thiếu dữ liệu
                var products = await _context.Products
                    .Include(p => p.Colors)
                    .Include(p => p.Sizes)
                    .Where(p => !p.Colors.Any() || !p.Sizes.Any())
                    .ToListAsync();

                foreach (var product in products)
                {
                    if (!product.Sizes.Any())
                    {
                        var defaultSizes = new[] { "S", "M", "L", "XL" };
                        foreach (var size in defaultSizes)
                        {
                            _context.SizeProducts.Add(new SizeProduct
                            {
                                ProductId = product.ProductId,
                                SizeName = size,
                                Stock = product.Stock / defaultSizes.Length
                            });
                        }
                        fixedCount++;
                    }
                    
                    if (!product.Colors.Any())
                    {
                        var defaultColors = new[] { "Đen", "Trắng", "Xanh" };
                        foreach (var color in defaultColors)
                        {
                            _context.ColorProducts.Add(new ColorProduct
                            {
                                ProductId = product.ProductId,
                                ColorName = color,
                                ColorCode = GetColorCode(color)
                            });
                        }
                        fixedCount++;
                    }
                }
            }

            await _context.SaveChangesAsync();
            
            return Json(new { success = true, message = $"Đã sửa {fixedCount} sản phẩm" });
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
        
        public async Task<IActionResult> FixBadProductName()
        {
            // Sửa sản phẩm có tên bị lặp lại
            var badProduct = await _context.Products.FindAsync(1035);
            if (badProduct != null)
            {
                badProduct.Name = "ĐẦM TUYTSY ĐEN CỔ VEST BUỘC DÂY EO";
                badProduct.Description = "Đầm tuytsy đen cổ vest buộc dây eo sang trọng";
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã sửa tên sản phẩm" });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }
    }
}