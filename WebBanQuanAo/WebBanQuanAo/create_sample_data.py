#!/usr/bin/env python3
"""
Tạo dữ liệu mẫu từ dữ liệu đã cào
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

def get_default_sizes():
    """Size mặc định"""
    return ['S', 'M', 'L', 'XL']

def get_default_colors():
    """Màu mặc định"""
    return ['Đen', 'Trắng', 'Be', 'Xanh', 'Hồng']

def create_sample_data():
    """Tạo dữ liệu mẫu"""
    scraped_data = load_scraped_data()
    
    if not scraped_data:
        print("Không tìm thấy dữ liệu đã cào!")
        return
    
    print(f"Đang xử lý {len(scraped_data)} sản phẩm...")
    
    # Tạo Categories
    categories = {}
    category_id = 1
    
    for item in scraped_data:
        cat_name = get_category_from_name(item['name'])
        if cat_name not in categories:
            categories[cat_name] = {
                'CategoryId': category_id,
                'Name': cat_name,
                'ImgUrl': item.get('image_url', ''),
                'CreatedAt': '2024-01-01T00:00:00'
            }
            category_id += 1
    
    # Tạo Products
    products = []
    colors = []
    sizes = []
    images = []
    
    color_id = 1
    size_id = 1
    image_id = 1
    
    for i, item in enumerate(scraped_data, 1):
        # Xác định category
        cat_name = get_category_from_name(item['name'])
        category_id = categories[cat_name]['CategoryId']
        
        # Tạo product
        price = extract_price_number(item.get('price', '0'))
        original_price = extract_price_number(item.get('original_price', '0'))
        
        product = {
            'ProductId': i,
            'Name': item['name'],
            'Description': item.get('description', f"Sản phẩm {item['name']} chất lượng cao, thiết kế hiện đại, phù hợp cho nhiều dịp khác nhau."),
            'Price': price if price > 0 else random.randint(500000, 2000000),
            'CategoryId': category_id,
            'IsNew': random.choice([True, False]),
            'IsTrend': item.get('is_sale', False),
            'CreatedAt': '2024-01-01T00:00:00'
        }
        products.append(product)
        
        # Tạo colors
        available_colors = item.get('available_colors', [])
        if not available_colors:
            available_colors = random.sample(get_default_colors(), random.randint(2, 4))
        
        for color_name in available_colors[:4]:  # Tối đa 4 màu
            color = {
                'ColorId': color_id,
                'ProductId': i,
                'ColorName': color_name,
                'ColorCode': f"#{random.randint(100000, 999999):06x}"
            }
            colors.append(color)
            color_id += 1
        
        # Tạo sizes
        available_sizes = item.get('available_sizes', [])
        if not available_sizes:
            available_sizes = get_default_sizes()
        
        for size_name in available_sizes:
            size = {
                'SizeId': size_id,
                'ProductId': i,
                'SizeName': size_name
            }
            sizes.append(size)
            size_id += 1
        
        # Tạo images
        all_images = item.get('all_images', [])
        if not all_images:
            all_images = [item.get('image_url', '')]
        
        for j, img_url in enumerate(all_images[:5]):  # Tối đa 5 ảnh
            if img_url:
                image = {
                    'ImageId': image_id,
                    'ProductId': i,
                    'ImageUrl': img_url,
                    'IsMain': j == 0
                }
                images.append(image)
                image_id += 1
    
    # Tạo sample data structure
    sample_data = {
        'Categories': list(categories.values()),
        'Products': products,
        'ColorProducts': colors,
        'SizeProducts': sizes,
        'ImageProducts': images
    }
    
    # Lưu ra file
    with open('sample_data.json', 'w', encoding='utf-8') as f:
        json.dump(sample_data, f, ensure_ascii=False, indent=2)
    
    # Tạo SQL Insert statements
    create_sql_inserts(sample_data)
    
    print(f"✅ Đã tạo dữ liệu mẫu:")
    print(f"   📁 sample_data.json")
    print(f"   📁 insert_sample_data.sql")
    print(f"   📊 {len(categories)} categories")
    print(f"   📦 {len(products)} products")
    print(f"   🎨 {len(colors)} colors")
    print(f"   📏 {len(sizes)} sizes")
    print(f"   📷 {len(images)} images")

def create_sql_inserts(data):
    """Tạo SQL Insert statements"""
    sql_content = "-- Dữ liệu mẫu từ scraper\n\n"
    
    # Categories
    sql_content += "-- Insert Categories\n"
    for cat in data['Categories']:
        sql_content += f"INSERT INTO Categories (CategoryId, Name, ImgUrl, CreatedAt) VALUES ({cat['CategoryId']}, N'{cat['Name']}', '{cat['ImgUrl']}', '{cat['CreatedAt']}');\n"
    
    sql_content += "\n-- Insert Products\n"
    for prod in data['Products']:
        desc = prod['Description'].replace("'", "''")  # Escape quotes
        sql_content += f"INSERT INTO Products (ProductId, Name, Description, Price, CategoryId, IsNew, IsTrend, CreatedAt) VALUES ({prod['ProductId']}, N'{prod['Name']}', N'{desc}', {prod['Price']}, {prod['CategoryId']}, {1 if prod['IsNew'] else 0}, {1 if prod['IsTrend'] else 0}, '{prod['CreatedAt']}');\n"
    
    sql_content += "\n-- Insert ColorProducts\n"
    for color in data['ColorProducts']:
        sql_content += f"INSERT INTO ColorProducts (ColorId, ProductId, ColorName, ColorCode) VALUES ({color['ColorId']}, {color['ProductId']}, N'{color['ColorName']}', '{color['ColorCode']}');\n"
    
    sql_content += "\n-- Insert SizeProducts\n"
    for size in data['SizeProducts']:
        sql_content += f"INSERT INTO SizeProducts (SizeId, ProductId, SizeName) VALUES ({size['SizeId']}, {size['ProductId']}, '{size['SizeName']}');\n"
    
    sql_content += "\n-- Insert ImageProducts\n"
    for img in data['ImageProducts']:
        sql_content += f"INSERT INTO ImageProducts (ImageId, ProductId, ImageUrl, IsMain) VALUES ({img['ImageId']}, {img['ProductId']}, '{img['ImageUrl']}', {1 if img['IsMain'] else 0});\n"
    
    with open('insert_sample_data.sql', 'w', encoding='utf-8') as f:
        f.write(sql_content)

if __name__ == "__main__":
    print("=== SAMPLE DATA CREATOR ===")
    print("Tạo dữ liệu mẫu từ dữ liệu đã cào")
    print()
    
    create_sample_data()
    
    input("\nNhấn Enter để thoát...")