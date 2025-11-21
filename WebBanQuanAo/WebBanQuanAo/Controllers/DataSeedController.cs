using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Models;
using System.Text.Json;

namespace WebBanQuanAo.Controllers
{
    public class SampleData
    {
        public List<SampleCategory> Categories { get; set; } = new();
        public List<SampleProduct> Products { get; set; } = new();
        public List<SampleColorProduct> ColorProducts { get; set; } = new();
        public List<SampleSizeProduct> SizeProducts { get; set; } = new();
        public List<SampleImageProduct> ImageProducts { get; set; } = new();
    }

    public class SampleCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImgUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SampleProduct
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsNew { get; set; }
        public bool IsTrend { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SampleColorProduct
    {
        public int ColorId { get; set; }
        public int ProductId { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
    }

    public class SampleSizeProduct
    {
        public int SizeId { get; set; }
        public int ProductId { get; set; }
        public string SizeName { get; set; } = string.Empty;
    }

    public class SampleImageProduct
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }

    public class DataSeedController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DataSeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> SeedData()
        {
            try
            {
                // Đọc dữ liệu từ file JSON
                var jsonPath = Path.Combine(_env.ContentRootPath, "sample_data.json");
                if (!System.IO.File.Exists(jsonPath))
                {
                    return Json(new { success = false, message = "Không tìm thấy file sample_data.json" });
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
                var sampleData = JsonSerializer.Deserialize<SampleData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (sampleData == null)
                {
                    return Json(new { success = false, message = "Không thể đọc dữ liệu từ file JSON" });
                }

                // Xóa dữ liệu cũ
                _context.ImageProducts.RemoveRange(_context.ImageProducts);
                _context.SizeProducts.RemoveRange(_context.SizeProducts);
                _context.ColorProducts.RemoveRange(_context.ColorProducts);
                _context.Products.RemoveRange(_context.Products);
                _context.Categories.RemoveRange(_context.Categories);
                _context.SaveChanges();

                // Tạo Categories từ JSON
                var categoryMap = new Dictionary<int, Category>();
                foreach (var cat in sampleData.Categories)
                {
                    var category = new Category
                    {
                        Name = cat.Name,
                        ImgUrl = cat.ImgUrl
                    };
                    _context.Categories.Add(category);
                    categoryMap[cat.CategoryId] = category;
                }

                _context.SaveChanges();

                // Tạo Products từ JSON
                var productMap = new Dictionary<int, Product>();
                foreach (var prod in sampleData.Products)
                {
                    var category = categoryMap.Values.FirstOrDefault(c => categoryMap.FirstOrDefault(x => x.Key == prod.CategoryId).Value == c);
                    if (category != null)
                    {
                        var product = new Product
                        {
                            Name = prod.Name,
                            Description = prod.Description,
                            Price = prod.Price,
                            CategoryId = category.CategoryId,
                            IsNew = prod.IsNew,
                            IsTrend = prod.IsTrend,
                            Stock = 100
                        };
                        _context.Products.Add(product);
                        productMap[prod.ProductId] = product;
                    }
                }

                _context.SaveChanges();

                // Tạo Colors từ JSON
                foreach (var colorData in sampleData.ColorProducts)
                {
                    var product = productMap.Values.FirstOrDefault(p => productMap.FirstOrDefault(x => x.Key == colorData.ProductId).Value == p);
                    if (product != null)
                    {
                        _context.ColorProducts.Add(new ColorProduct
                        {
                            ProductId = product.ProductId,
                            ColorName = colorData.ColorName,
                            ColorCode = colorData.ColorCode
                        });
                    }
                }

                // Tạo Sizes từ JSON
                foreach (var sizeData in sampleData.SizeProducts)
                {
                    var product = productMap.Values.FirstOrDefault(p => productMap.FirstOrDefault(x => x.Key == sizeData.ProductId).Value == p);
                    if (product != null)
                    {
                        _context.SizeProducts.Add(new SizeProduct
                        {
                            ProductId = product.ProductId,
                            SizeName = sizeData.SizeName
                        });
                    }
                }

                // Tạo Images từ JSON
                foreach (var imageData in sampleData.ImageProducts)
                {
                    var product = productMap.Values.FirstOrDefault(p => productMap.FirstOrDefault(x => x.Key == imageData.ProductId).Value == p);
                    if (product != null)
                    {
                        _context.ImageProducts.Add(new ImageProduct
                        {
                            ProductId = product.ProductId,
                            ImageUrl = imageData.ImageUrl,
                            IsMain = imageData.IsMain
                        });
                    }
                }

                _context.SaveChanges();

                return Json(new { 
                    success = true, 
                    message = $"Đã tạo dữ liệu mẫu thành công! {sampleData.Categories.Count} categories, {sampleData.Products.Count} products" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}