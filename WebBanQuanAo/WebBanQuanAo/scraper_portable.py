#!/usr/bin/env python3
"""
Portable Fashion Scraper - Không cần cài đặt thư viện
Sử dụng thư viện có sẵn trong Python
"""

import urllib.request
import urllib.parse
import json
import csv
import re
import time
import html.parser
from dataclasses import dataclass
from typing import List, Dict, Optional

class SimpleHTMLParser(html.parser.HTMLParser):
    """Parser HTML đơn giản"""
    def __init__(self):
        super().__init__()
        self.data = []
        self.current_tag = None
        self.current_attrs = {}
        
    def handle_starttag(self, tag, attrs):
        self.current_tag = tag
        self.current_attrs = dict(attrs)
        
    def handle_data(self, data):
        if self.current_tag and data.strip():
            self.data.append({
                'tag': self.current_tag,
                'attrs': self.current_attrs,
                'text': data.strip()
            })

@dataclass
class Product:
    """Class để lưu thông tin sản phẩm"""
    id: str
    name: str
    price: str
    original_price: str
    discount_percent: str
    image_url: str
    product_url: str
    is_sale: bool

class PortableFashionScraper:
    def __init__(self, base_url: str = "https://elise.vn"):
        self.base_url = base_url
        
    def get_page(self, url: str) -> Optional[str]:
        """Lấy nội dung trang web"""
        try:
            headers = {
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
            }
            
            req = urllib.request.Request(url, headers=headers)
            with urllib.request.urlopen(req, timeout=10) as response:
                return response.read().decode('utf-8')
        except Exception as e:
            print(f"Lỗi khi lấy trang {url}: {e}")
            return None
    
    def extract_products_simple(self, html_content: str) -> List[Product]:
        """Trích xuất sản phẩm bằng regex đơn giản"""
        products = []
        
        # Pattern để tìm sản phẩm
        product_pattern = r'<li class="item product product-item-info product-item[^"]*"[^>]*>(.*?)</li>'
        product_matches = re.findall(product_pattern, html_content, re.DOTALL)
        
        for match in product_matches:
            try:
                # Tên sản phẩm
                name_pattern = r'<a class="product-item-link"[^>]*>([^<]+)</a>'
                name_match = re.search(name_pattern, match)
                name = name_match.group(1).strip() if name_match else ""
                
                # URL sản phẩm
                url_pattern = r'<a class="product-item-link" href="([^"]+)"'
                url_match = re.search(url_pattern, match)
                product_url = url_match.group(1) if url_match else ""
                if product_url and not product_url.startswith('http'):
                    product_url = self.base_url + product_url
                
                # ID sản phẩm
                id_pattern = r'data-product-id="([^"]+)"'
                id_match = re.search(id_pattern, match)
                product_id = id_match.group(1) if id_match else ""
                
                # Giá đặc biệt
                price_pattern = r'<span class="special-price">.*?<span class="price">([^<]+)</span>'
                price_match = re.search(price_pattern, match, re.DOTALL)
                price = price_match.group(1).strip() if price_match else ""
                
                # Giá gốc
                old_price_pattern = r'<span class="old-price">.*?<span class="price">([^<]+)</span>'
                old_price_match = re.search(old_price_pattern, match, re.DOTALL)
                original_price = old_price_match.group(1).strip() if old_price_match else ""
                
                # Hình ảnh
                img_pattern = r'data-src="([^"]+)"'
                img_match = re.search(img_pattern, match)
                image_url = img_match.group(1) if img_match else ""
                if image_url and not image_url.startswith('http'):
                    image_url = self.base_url + image_url
                
                # Sale
                is_sale = 'sale-label' in match
                
                # Tính phần trăm giảm giá
                discount_percent = ""
                if price and original_price:
                    try:
                        price_num = float(re.sub(r'[^\d.]', '', price.replace('.', '').replace(',', '.')))
                        original_num = float(re.sub(r'[^\d.]', '', original_price.replace('.', '').replace(',', '.')))
                        if original_num > 0:
                            discount = ((original_num - price_num) / original_num) * 100
                            discount_percent = f"{discount:.0f}%"
                    except:
                        pass
                
                if name:  # Chỉ thêm nếu có tên
                    product = Product(
                        id=product_id,
                        name=name,
                        price=price,
                        original_price=original_price,
                        discount_percent=discount_percent,
                        image_url=image_url,
                        product_url=product_url,
                        is_sale=is_sale
                    )
                    products.append(product)
                    
            except Exception as e:
                print(f"Lỗi khi xử lý sản phẩm: {e}")
                continue
        
        return products
    
    def scrape_homepage(self) -> List[Product]:
        """Cào dữ liệu từ trang chủ"""
        print("Đang cào dữ liệu từ trang chủ...")
        
        html_content = self.get_page(f"{self.base_url}/thoi-trang-nu")
        if not html_content:
            print("Không thể lấy nội dung trang web!")
            return []
        
        products = self.extract_products_simple(html_content)
        print(f"Tìm thấy {len(products)} sản phẩm")
        
        for product in products:
            print(f"Đã cào: {product.name}")
        
        return products
    
    def save_to_json(self, products: List[Product], filename: str = "products_portable.json"):
        """Lưu dữ liệu ra file JSON"""
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
                'is_sale': product.is_sale
            })
        
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        
        print(f"Đã lưu {len(products)} sản phẩm vào {filename}")
    
    def save_to_csv(self, products: List[Product], filename: str = "products_portable.csv"):
        """Lưu dữ liệu ra file CSV"""
        with open(filename, 'w', newline='', encoding='utf-8') as f:
            writer = csv.writer(f)
            
            # Header
            writer.writerow([
                'ID', 'Tên sản phẩm', 'Giá', 'Giá gốc', '% Giảm giá',
                'Hình ảnh', 'URL', 'Sale'
            ])
            
            # Data
            for product in products:
                writer.writerow([
                    product.id,
                    product.name,
                    product.price,
                    product.original_price,
                    product.discount_percent,
                    product.image_url,
                    product.product_url,
                    'Có' if product.is_sale else 'Không'
                ])
        
        print(f"Đã lưu {len(products)} sản phẩm vào {filename}")

def main():
    """Hàm chính"""
    print("=== PORTABLE FASHION SCRAPER ===")
    print("Phiên bản không cần cài đặt thư viện bên ngoài")
    print()
    
    scraper = PortableFashionScraper()
    
    try:
        # Cào dữ liệu
        products = scraper.scrape_homepage()
        
        if products:
            # Lưu ra JSON
            scraper.save_to_json(products)
            
            # Lưu ra CSV
            scraper.save_to_csv(products)
            
            print(f"\n✅ Đã cào thành công {len(products)} sản phẩm!")
            print("📁 File đã tạo:")
            print("   - products_portable.json")
            print("   - products_portable.csv")
        else:
            print("❌ Không cào được sản phẩm nào!")
            
    except Exception as e:
        print(f"❌ Lỗi: {e}")
    
    input("\nNhấn Enter để thoát...")

if __name__ == "__main__":
    main()