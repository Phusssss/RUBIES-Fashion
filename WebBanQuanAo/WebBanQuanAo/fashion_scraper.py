#!/usr/bin/env python3
"""
Fashion Website Scraper Tool
Cào dữ liệu từ trang web thời trang Elise
"""

import requests
from bs4 import BeautifulSoup
import json
import csv
import re
import time
from urllib.parse import urljoin, urlparse
import os
from dataclasses import dataclass
from typing import List, Dict, Optional

@dataclass
class Product:
    """Class để lưu thông tin sản phẩm"""
    id: str
    name: str
    price: str
    original_price: str
    discount_percent: str
    image_url: str
    hover_image_url: str
    product_url: str
    category: str
    is_sale: bool

class FashionScraper:
    def __init__(self, base_url: str = "https://elise.vn"):
        self.base_url = base_url
        self.session = requests.Session()
        self.session.headers.update({
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
        })
        
    def get_page(self, url: str) -> Optional[BeautifulSoup]:
        """Lấy nội dung trang web"""
        try:
            response = self.session.get(url, timeout=10)
            response.raise_for_status()
            return BeautifulSoup(response.content, 'html.parser')
        except Exception as e:
            print(f"Lỗi khi lấy trang {url}: {e}")
            return None
    
    def extract_product_info(self, product_element) -> Optional[Product]:
        """Trích xuất thông tin sản phẩm từ HTML element"""
        try:
            # Lấy ID sản phẩm
            product_id = ""
            price_box = product_element.find('div', {'data-product-id': True})
            if price_box:
                product_id = price_box.get('data-product-id', '')
            
            # Lấy tên sản phẩm
            name_element = product_element.find('h5', class_='product name product-item-name')
            name = name_element.find('a').get_text(strip=True) if name_element else ""
            
            # Lấy URL sản phẩm
            product_url = ""
            link_element = product_element.find('a', class_='product-item-link')
            if link_element:
                product_url = urljoin(self.base_url, link_element.get('href', ''))
            
            # Lấy giá
            price = ""
            original_price = ""
            discount_percent = ""
            
            # Giá đặc biệt
            special_price = product_element.find('span', class_='special-price')
            if special_price:
                price_span = special_price.find('span', class_='price')
                if price_span:
                    price = price_span.get_text(strip=True)
            
            # Giá gốc
            old_price = product_element.find('span', class_='old-price')
            if old_price:
                price_span = old_price.find('span', class_='price')
                if price_span:
                    original_price = price_span.get_text(strip=True)
            
            # Tính phần trăm giảm giá
            if price and original_price:
                try:
                    price_num = float(re.sub(r'[^\d.]', '', price.replace('.', '').replace(',', '.')))
                    original_num = float(re.sub(r'[^\d.]', '', original_price.replace('.', '').replace(',', '.')))
                    if original_num > 0:
                        discount = ((original_num - price_num) / original_num) * 100
                        discount_percent = f"{discount:.0f}%"
                except:
                    pass
            
            # Lấy hình ảnh
            image_url = ""
            hover_image_url = ""
            
            img_main = product_element.find('img', class_='product-image-photo')
            if img_main:
                image_url = img_main.get('data-src') or img_main.get('src', '')
                if image_url:
                    image_url = urljoin(self.base_url, image_url)
            
            img_hover = product_element.find('img', class_='product-image-photo-hover')
            if img_hover:
                hover_image_url = img_hover.get('data-src') or img_hover.get('src', '')
                if hover_image_url:
                    hover_image_url = urljoin(self.base_url, hover_image_url)
            
            # Kiểm tra có sale không
            is_sale = bool(product_element.find('span', class_='product-label sale-label'))
            
            # Danh mục (có thể lấy từ URL hoặc breadcrumb)
            category = "Thời trang nữ"  # Mặc định
            
            return Product(
                id=product_id,
                name=name,
                price=price,
                original_price=original_price,
                discount_percent=discount_percent,
                image_url=image_url,
                hover_image_url=hover_image_url,
                product_url=product_url,
                category=category,
                is_sale=is_sale
            )
            
        except Exception as e:
            print(f"Lỗi khi trích xuất thông tin sản phẩm: {e}")
            return None
    
    def scrape_homepage(self) -> List[Product]:
        """Cào dữ liệu từ trang chủ"""
        print("Đang cào dữ liệu từ trang chủ...")
        
        soup = self.get_page(f"{self.base_url}/thoi-trang-nu")
        if not soup:
            return []
        
        products = []
        
        # Tìm tất cả sản phẩm
        product_items = soup.find_all('li', class_='item product product-item-info product-item')
        
        print(f"Tìm thấy {len(product_items)} sản phẩm")
        
        for item in product_items:
            product = self.extract_product_info(item)
            if product and product.name:
                products.append(product)
                print(f"Đã cào: {product.name}")
        
        return products
    
    def scrape_product_detail(self, product_url: str) -> Dict:
        """Cào chi tiết sản phẩm"""
        print(f"Đang cào chi tiết sản phẩm: {product_url}")
        
        soup = self.get_page(product_url)
        if not soup:
            return {}
        
        detail = {}
        
        try:
            # Tên sản phẩm
            title = soup.find('h1', class_='page-title')
            if title:
                detail['name'] = title.get_text(strip=True)
            
            # Mô tả
            description = soup.find('div', class_='product attribute description')
            if description:
                detail['description'] = description.get_text(strip=True)
            
            # Thông số kỹ thuật
            attributes = soup.find('div', class_='product attribute overview')
            if attributes:
                detail['attributes'] = attributes.get_text(strip=True)
            
            # Hình ảnh chi tiết
            images = []
            gallery = soup.find('div', class_='fotorama')
            if gallery:
                img_tags = gallery.find_all('img')
                for img in img_tags:
                    img_url = img.get('data-src') or img.get('src')
                    if img_url:
                        images.append(urljoin(self.base_url, img_url))
            
            detail['images'] = images
            
            # Kích thước có sẵn
            sizes = []
            size_options = soup.find_all('div', class_='swatch-option')
            for option in size_options:
                size_text = option.get_text(strip=True)
                if size_text:
                    sizes.append(size_text)
            
            detail['sizes'] = sizes
            
        except Exception as e:
            print(f"Lỗi khi cào chi tiết sản phẩm: {e}")
        
        return detail
    
    def save_to_json(self, products: List[Product], filename: str = "products.json"):
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
                'hover_image_url': product.hover_image_url,
                'product_url': product.product_url,
                'category': product.category,
                'is_sale': product.is_sale
            })
        
        with open(filename, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        
        print(f"Đã lưu {len(products)} sản phẩm vào {filename}")
    
    def save_to_csv(self, products: List[Product], filename: str = "products.csv"):
        """Lưu dữ liệu ra file CSV"""
        with open(filename, 'w', newline='', encoding='utf-8') as f:
            writer = csv.writer(f)
            
            # Header
            writer.writerow([
                'ID', 'Tên sản phẩm', 'Giá', 'Giá gốc', '% Giảm giá',
                'Hình ảnh', 'Hình hover', 'URL', 'Danh mục', 'Sale'
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
                    product.hover_image_url,
                    product.product_url,
                    product.category,
                    'Có' if product.is_sale else 'Không'
                ])
        
        print(f"Đã lưu {len(products)} sản phẩm vào {filename}")
    
    def scrape_with_details(self, max_products: int = 10) -> List[Dict]:
        """Cào sản phẩm kèm chi tiết"""
        products = self.scrape_homepage()
        detailed_products = []
        
        for i, product in enumerate(products[:max_products]):
            print(f"Đang cào chi tiết sản phẩm {i+1}/{min(max_products, len(products))}")
            
            detail = self.scrape_product_detail(product.product_url)
            
            product_dict = {
                'id': product.id,
                'name': product.name,
                'price': product.price,
                'original_price': product.original_price,
                'discount_percent': product.discount_percent,
                'image_url': product.image_url,
                'hover_image_url': product.hover_image_url,
                'product_url': product.product_url,
                'category': product.category,
                'is_sale': product.is_sale,
                'detail': detail
            }
            
            detailed_products.append(product_dict)
            
            # Nghỉ 1 giây để tránh spam
            time.sleep(1)
        
        return detailed_products

def main():
    """Hàm chính"""
    scraper = FashionScraper()
    
    print("=== FASHION SCRAPER TOOL ===")
    print("1. Cào danh sách sản phẩm từ trang chủ")
    print("2. Cào sản phẩm kèm chi tiết (chậm hơn)")
    print("3. Thoát")
    
    choice = input("Chọn chức năng (1-3): ").strip()
    
    if choice == "1":
        # Cào danh sách sản phẩm
        products = scraper.scrape_homepage()
        
        if products:
            # Lưu ra JSON
            scraper.save_to_json(products, "elise_products.json")
            
            # Lưu ra CSV
            scraper.save_to_csv(products, "elise_products.csv")
            
            print(f"\nĐã cào thành công {len(products)} sản phẩm!")
        else:
            print("Không cào được sản phẩm nào!")
    
    elif choice == "2":
        # Cào kèm chi tiết
        max_products = int(input("Nhập số sản phẩm muốn cào chi tiết (khuyến nghị <= 10): ") or "5")
        
        detailed_products = scraper.scrape_with_details(max_products)
        
        if detailed_products:
            with open("elise_products_detailed.json", 'w', encoding='utf-8') as f:
                json.dump(detailed_products, f, ensure_ascii=False, indent=2)
            
            print(f"\nĐã cào thành công {len(detailed_products)} sản phẩm kèm chi tiết!")
        else:
            print("Không cào được sản phẩm nào!")
    
    elif choice == "3":
        print("Tạm biệt!")
    
    else:
        print("Lựa chọn không hợp lệ!")

if __name__ == "__main__":
    main()