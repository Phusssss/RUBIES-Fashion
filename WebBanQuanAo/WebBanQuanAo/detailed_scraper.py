#!/usr/bin/env python3
"""
Detailed Fashion Scraper - Cào chi tiết từng sản phẩm
"""

import urllib.request
import json
import re
import time
from dataclasses import dataclass
from typing import List, Dict, Optional

@dataclass
class ProductDetail:
    """Chi tiết sản phẩm"""
    id: str
    name: str
    price: str
    original_price: str
    discount_percent: str
    image_url: str
    product_url: str
    is_sale: bool
    # Chi tiết mới
    description: str
    material: str
    size_guide: str
    care_instructions: str
    all_images: List[str]
    available_sizes: List[str]
    available_colors: List[str]
    sku: str
    brand: str

class DetailedScraper:
    def __init__(self):
        self.base_url = "https://elise.vn"
        
    def get_page(self, url: str) -> Optional[str]:
        """Lấy nội dung trang web"""
        try:
            headers = {
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
            }
            
            req = urllib.request.Request(url, headers=headers)
            with urllib.request.urlopen(req, timeout=15) as response:
                return response.read().decode('utf-8')
        except Exception as e:
            print(f"Lỗi khi lấy trang {url}: {e}")
            return None
    
    def extract_product_detail(self, html_content: str, product_url: str) -> Dict:
        """Trích xuất chi tiết sản phẩm"""
        detail = {}
        
        try:
            # Mô tả sản phẩm
            desc_patterns = [
                r'<div[^>]*class="[^"]*product attribute description[^"]*"[^>]*>(.*?)</div>',
                r'<div[^>]*class="[^"]*value[^"]*"[^>]*>(.*?)</div>',
                r'<div[^>]*id="description"[^>]*>(.*?)</div>'
            ]
            
            description = ""
            for pattern in desc_patterns:
                match = re.search(pattern, html_content, re.DOTALL | re.IGNORECASE)
                if match:
                    desc_text = re.sub(r'<[^>]+>', ' ', match.group(1))
                    desc_text = re.sub(r'\s+', ' ', desc_text).strip()
                    if len(desc_text) > 50:  # Chỉ lấy mô tả có ý nghĩa
                        description = desc_text[:500] + "..." if len(desc_text) > 500 else desc_text
                        break
            
            detail['description'] = description
            
            # Chất liệu
            material_patterns = [
                r'Chất liệu[:\s]*([^<\n]+)',
                r'Material[:\s]*([^<\n]+)',
                r'Thành phần[:\s]*([^<\n]+)'
            ]
            
            material = ""
            for pattern in material_patterns:
                match = re.search(pattern, html_content, re.IGNORECASE)
                if match:
                    material = match.group(1).strip()
                    break
            
            detail['material'] = material
            
            # Hướng dẫn bảo quản
            care_patterns = [
                r'Hướng dẫn bảo quản[:\s]*([^<\n]+)',
                r'Care instructions[:\s]*([^<\n]+)',
                r'Bảo quản[:\s]*([^<\n]+)'
            ]
            
            care_instructions = ""
            for pattern in care_patterns:
                match = re.search(pattern, html_content, re.IGNORECASE)
                if match:
                    care_instructions = match.group(1).strip()
                    break
            
            detail['care_instructions'] = care_instructions
            
            # Tất cả hình ảnh
            img_patterns = [
                r'data-src="([^"]*\.jpg[^"]*)"',
                r'src="([^"]*\.jpg[^"]*)"',
                r'data-zoom-image="([^"]*\.jpg[^"]*)"'
            ]
            
            all_images = set()
            for pattern in img_patterns:
                matches = re.findall(pattern, html_content)
                for match in matches:
                    if 'catalog/product' in match:
                        if not match.startswith('http'):
                            match = self.base_url + match
                        all_images.add(match)
            
            detail['all_images'] = list(all_images)[:10]  # Giới hạn 10 ảnh
            
            # Kích thước có sẵn
            size_patterns = [
                r'<div[^>]*class="[^"]*swatch-option[^"]*"[^>]*[^>]*>([^<]*)</div>',
                r'option-label-size-[^>]*>([^<]+)<',
                r'data-option-label="([^"]*)"'
            ]
            
            available_sizes = set()
            for pattern in size_patterns:
                matches = re.findall(pattern, html_content)
                for match in matches:
                    size = match.strip()
                    if size and len(size) <= 10 and any(c.isalnum() for c in size):
                        available_sizes.add(size)
            
            detail['available_sizes'] = list(available_sizes)
            
            # Màu sắc có sẵn
            color_patterns = [
                r'option-label-color-[^>]*>([^<]+)<',
                r'data-option-label="([^"]*)"[^>]*color',
                r'swatch-option color[^>]*title="([^"]*)"'
            ]
            
            available_colors = set()
            for pattern in color_patterns:
                matches = re.findall(pattern, html_content)
                for match in matches:
                    color = match.strip()
                    if color and len(color) <= 20:
                        available_colors.add(color)
            
            detail['available_colors'] = list(available_colors)
            
            # SKU
            sku_patterns = [
                r'SKU[:\s]*([A-Z0-9]+)',
                r'Mã sản phẩm[:\s]*([A-Z0-9]+)',
                r'"sku":"([^"]+)"'
            ]
            
            sku = ""
            for pattern in sku_patterns:
                match = re.search(pattern, html_content, re.IGNORECASE)
                if match:
                    sku = match.group(1).strip()
                    break
            
            detail['sku'] = sku
            
            # Thương hiệu
            detail['brand'] = "ELISE"
            
            # Hướng dẫn size
            size_guide = ""
            if 'size' in html_content.lower() or 'kích thước' in html_content.lower():
                size_guide = "Vui lòng tham khảo bảng size trên website"
            
            detail['size_guide'] = size_guide
            
        except Exception as e:
            print(f"Lỗi khi trích xuất chi tiết: {e}")
        
        return detail
    
    def load_products_from_json(self, filename: str = "products_portable.json") -> List[Dict]:
        """Đọc danh sách sản phẩm từ file JSON"""
        try:
            with open(filename, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            print(f"Lỗi khi đọc file {filename}: {e}")
            return []
    
    def scrape_product_details(self, products: List[Dict], max_products: int = 10) -> List[ProductDetail]:
        """Cào chi tiết từng sản phẩm"""
        detailed_products = []
        
        print(f"Bắt đầu cào chi tiết {min(max_products, len(products))} sản phẩm...")
        
        for i, product in enumerate(products[:max_products]):
            print(f"\n[{i+1}/{min(max_products, len(products))}] Đang cào: {product['name']}")
            
            # Lấy HTML của trang chi tiết
            html_content = self.get_page(product['product_url'])
            if not html_content:
                print("  ❌ Không thể lấy nội dung trang")
                continue
            
            # Trích xuất chi tiết
            detail = self.extract_product_detail(html_content, product['product_url'])
            
            # Tạo object ProductDetail
            detailed_product = ProductDetail(
                id=product.get('id', ''),
                name=product.get('name', ''),
                price=product.get('price', ''),
                original_price=product.get('original_price', ''),
                discount_percent=product.get('discount_percent', ''),
                image_url=product.get('image_url', ''),
                product_url=product.get('product_url', ''),
                is_sale=product.get('is_sale', False),
                description=detail.get('description', ''),
                material=detail.get('material', ''),
                size_guide=detail.get('size_guide', ''),
                care_instructions=detail.get('care_instructions', ''),
                all_images=detail.get('all_images', []),
                available_sizes=detail.get('available_sizes', []),
                available_colors=detail.get('available_colors', []),
                sku=detail.get('sku', ''),
                brand=detail.get('brand', 'ELISE')
            )
            
            detailed_products.append(detailed_product)
            
            # Hiển thị thông tin đã cào được
            print(f"  ✅ Mô tả: {detail['description'][:100]}..." if detail['description'] else "  ⚠️ Không có mô tả")
            print(f"  📷 Hình ảnh: {len(detail['all_images'])} ảnh")
            print(f"  📏 Size: {', '.join(detail['available_sizes'][:5])}" if detail['available_sizes'] else "  📏 Không có thông tin size")
            print(f"  🎨 Màu: {', '.join(detail['available_colors'][:3])}" if detail['available_colors'] else "  🎨 Không có thông tin màu")
            
            # Nghỉ 2 giây để tránh spam
            time.sleep(2)
        
        return detailed_products
    
    def save_detailed_products(self, products: List[ProductDetail], filename: str = "products_detailed.json"):
        """Lưu sản phẩm chi tiết ra file JSON"""
        data = []
        for product in products:
            data.append({
                'id': product.id,
                'name': product.name,
                'price': product.price,
                'original_price': product.original_price,
                'discount_percent': product.discount_percent,
                'image_url': product.image_url,
                'product_url': product.product_url,
                'is_sale': product.is_sale,
                'description': product.description,
                'material': product.material,
                'size_guide': product.size_guide,
                'care_instructions': product.care_instructions,
                'all_images': product.all_images,
                'available_sizes': product.available_sizes,
                'available_colors': product.available_colors,
                'sku': product.sku,
                'brand': product.brand
            })
        
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        
        print(f"\n💾 Đã lưu {len(products)} sản phẩm chi tiết vào {filename}")

def main():
    """Hàm chính"""
    print("=== DETAILED PRODUCT SCRAPER ===")
    print("Cào chi tiết từng sản phẩm từ file products_portable.json")
    print()
    
    scraper = DetailedScraper()
    
    # Đọc danh sách sản phẩm
    products = scraper.load_products_from_json()
    if not products:
        print("❌ Không tìm thấy file products_portable.json hoặc file rỗng!")
        print("Vui lòng chạy scraper_portable.py trước!")
        input("Nhấn Enter để thoát...")
        return
    
    print(f"📋 Tìm thấy {len(products)} sản phẩm trong file JSON")
    
    # Hỏi số lượng sản phẩm muốn cào chi tiết
    try:
        max_products = input(f"Nhập số sản phẩm muốn cào chi tiết (1-{len(products)}, mặc định 5): ").strip()
        max_products = int(max_products) if max_products else 5
        max_products = min(max_products, len(products))
    except:
        max_products = 5
    
    print(f"🎯 Sẽ cào chi tiết {max_products} sản phẩm đầu tiên")
    print("⏱️ Ước tính thời gian: ~{} phút".format(max_products * 2 // 60 + 1))
    
    confirm = input("Tiếp tục? (y/n): ").strip().lower()
    if confirm not in ['y', 'yes', '']:
        print("Đã hủy!")
        return
    
    try:
        # Cào chi tiết
        detailed_products = scraper.scrape_product_details(products, max_products)
        
        if detailed_products:
            # Lưu kết quả
            scraper.save_detailed_products(detailed_products)
            
            print(f"\n🎉 Hoàn thành! Đã cào chi tiết {len(detailed_products)} sản phẩm")
            print("📁 File đã tạo: products_detailed.json")
        else:
            print("❌ Không cào được chi tiết sản phẩm nào!")
            
    except KeyboardInterrupt:
        print("\n⏹️ Đã dừng bởi người dùng!")
    except Exception as e:
        print(f"❌ Lỗi: {e}")
    
    input("\nNhấn Enter để thoát...")

if __name__ == "__main__":
    main()