#!/usr/bin/env python3
"""
Cập nhật DataSeedController với dữ liệu đã cào
"""

import json
import random
import re

def load_scraped_data():
    """Đọc dữ liệu đã cào"""
    try:
        with open('products_detailed.json', 'r', encoding='utf-8') as f:
            return json.load(f)
    except:
        try:
            with open('products_portable.json', 'r', encoding='utf-8') as f:
                return json.load(f)
        except:
            return []

def extract_price_number(price_str):
    """Trích xuất số từ chuỗi giá"""
    if not price_str:
        return 0
    numbers = re.findall(r'[\d.]+', price_str.replace('.', '').replace(',', ''))
    return int(numbers[0]) if numbers else 0

def get_category_from_name(name):
    """Xác định danh mục từ tên sản phẩm"""
    name_lower = name.lower()
    if any(word in name_lower for word in ['đầm', 'dam']):
        return 'Đầm'
    elif any(word in name_lower for word in ['quần', 'quan']):
        return 'Quần'
    elif any(word in name_lower for word in ['áo', 'ao', 'vest', 'sm']):
        return 'Áo'
    elif any(word in name_lower for word in ['chân váy', 'cv', 'jupe']):
        return 'Chân Váy'
    else:
        return 'Thời trang nữ'

def generate_controller_code():
    """Tạo code cho DataSeedController"""
    scraped_data = load_scraped_data()
    
    if not scraped_data:
        print("Không tìm thấy dữ liệu đã cào!")
        return
    
    print(f"Đang tạo code từ {len(scraped_data)} sản phẩm...")
    
    # Tạo categories
    categories = {}
    for item in scraped_data:
        cat_name = get_category_from_name(item['name'])
        if cat_name not in categories:
            categories[cat_name] = {
                'name': cat_name,
                'img': item.get('image_url', '')
            }
    
    # Tạo code
    code = '''using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class DataSeedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DataSeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SeedData()
        {
            // Xóa dữ liệu cũ
            _context.ImageProducts.RemoveRange(_context.ImageProducts);
            _context.SizeProducts.RemoveRange(_context.SizeProducts);
            _context.ColorProducts.RemoveRange(_context.ColorProducts);
            _context.Products.RemoveRange(_context.Products);
            _context.Categories.RemoveRange(_context.Categories);
            _context.SaveChanges();

            // Tạo Categories
'''
    
    # Thêm categories
    for i, (cat_name, cat_data) in enumerate(categories.items(), 1):
        code += f'''            var category{i} = new Category
            {{
                Name = "{cat_name}",
                ImgUrl = "{cat_data['img']}",
                CreatedAt = DateTime.Now
            }};
            _context.Categories.Add(category{i});
'''
    
    code += '''
            _context.SaveChanges();

            // Tạo Products
'''
    
    # Thêm products
    for i, item in enumerate(scraped_data, 1):
        cat_name = get_category_from_name(item['name'])
        cat_var = f"category{list(categories.keys()).index(cat_name) + 1}"
        
        price = extract_price_number(item.get('price', '0'))
        if price == 0:
            price = random.randint(500000, 2000000)
        
        description = item.get('description', f"Sản phẩm {item['name']} chất lượng cao")
        description = description.replace('"', '\\"').replace('\n', ' ')[:200]
        
        code += f'''            var product{i} = new Product
            {{
                Name = "{item['name']}",
                Description = "{description}",
                Price = {price},
                CategoryId = {cat_var}.CategoryId,
                IsNew = {str(random.choice([True, False])).lower()},
                IsTrend = {str(item.get('is_sale', False)).lower()},
                CreatedAt = DateTime.Now
            }};
            _context.Products.Add(product{i});
'''
    
    code += '''
            _context.SaveChanges();

            // Tạo Colors và Sizes
'''
    
    # Thêm colors và sizes
    default_colors = ['Đen', 'Trắng', 'Be', 'Xanh', 'Hồng']
    default_sizes = ['S', 'M', 'L', 'XL']
    
    for i, item in enumerate(scraped_data, 1):
        # Colors
        available_colors = item.get('available_colors', [])
        if not available_colors:
            available_colors = random.sample(default_colors, random.randint(2, 3))
        
        for color in available_colors[:3]:
            code += f'''            _context.ColorProducts.Add(new ColorProduct {{ ProductId = product{i}.ProductId, ColorName = "{color}", ColorCode = "#{random.randint(100000, 999999):06x}" }});
'''
        
        # Sizes
        available_sizes = item.get('available_sizes', [])
        if not available_sizes:
            available_sizes = default_sizes
        
        for size in available_sizes:
            code += f'''            _context.SizeProducts.Add(new SizeProduct {{ ProductId = product{i}.ProductId, SizeName = "{size}" }});
'''
        
        # Images
        all_images = item.get('all_images', [])
        if not all_images:
            all_images = [item.get('image_url', '')]
        
        for j, img_url in enumerate(all_images[:3]):
            if img_url:
                code += f'''            _context.ImageProducts.Add(new ImageProduct {{ ProductId = product{i}.ProductId, ImageUrl = "{img_url}", IsMain = {str(j == 0).lower()} }});
'''
    
    code += '''
            _context.SaveChanges();

            return Json(new { success = true, message = "Đã tạo dữ liệu mẫu thành công!" });
        }
    }
}'''
    
    # Lưu file
    with open('DataSeedController_Updated.cs', 'w', encoding='utf-8') as f:
        f.write(code)
    
    print(f"✅ Đã tạo DataSeedController_Updated.cs")
    print(f"   📊 {len(categories)} categories")
    print(f"   📦 {len(scraped_data)} products")
    print(f"   🎨 Colors và sizes tự động")
    print(f"   📷 Images từ dữ liệu đã cào")

if __name__ == "__main__":
    print("=== DATA SEED CONTROLLER UPDATER ===")
    print("Cập nhật DataSeedController với dữ liệu đã cào")
    print()
    
    generate_controller_code()
    
    input("\nNhấn Enter để thoát...")