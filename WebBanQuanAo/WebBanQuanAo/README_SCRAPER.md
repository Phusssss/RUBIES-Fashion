# Fashion Website Scraper Tool

Tool Python để cào dữ liệu từ trang web thời trang Elise.vn

## Tính năng

- ✅ Cào danh sách sản phẩm từ trang chủ
- ✅ Trích xuất thông tin chi tiết sản phẩm
- ✅ Lưu dữ liệu ra file JSON và CSV
- ✅ Xử lý hình ảnh và giá cả
- ✅ Phát hiện sản phẩm sale
- ✅ Tính phần trăm giảm giá

## Cài đặt

1. Cài đặt Python 3.7+ trên máy tính
2. Cài đặt các thư viện cần thiết:
```bash
pip install -r requirements.txt
```

## Cách sử dụng

### Cách 1: Chạy trực tiếp
```bash
python fashion_scraper.py
```

### Cách 2: Chạy bằng file batch (Windows)
```bash
run_scraper.bat
```

## Chức năng

### 1. Cào danh sách sản phẩm
- Cào tất cả sản phẩm từ trang chủ
- Lưu ra file `elise_products.json` và `elise_products.csv`
- Nhanh và hiệu quả

### 2. Cào sản phẩm kèm chi tiết
- Cào thông tin chi tiết từng sản phẩm
- Bao gồm mô tả, hình ảnh chi tiết, kích thước
- Chậm hơn nhưng đầy đủ thông tin hơn
- Lưu ra file `elise_products_detailed.json`

## Dữ liệu thu thập

### Thông tin cơ bản:
- ID sản phẩm
- Tên sản phẩm
- Giá hiện tại
- Giá gốc
- Phần trăm giảm giá
- Hình ảnh chính
- Hình ảnh hover
- URL sản phẩm
- Danh mục
- Trạng thái sale

### Thông tin chi tiết (chế độ 2):
- Mô tả sản phẩm
- Thông số kỹ thuật
- Tất cả hình ảnh
- Kích thước có sẵn

## Cấu trúc file output

### JSON Format:
```json
{
  "id": "73418",
  "name": "ĐẦM TUYTSY KẺ TRẮNG NỀN ĐEN NHÚN EO",
  "price": "1.608.600 VND",
  "original_price": "2.298.000 VND",
  "discount_percent": "30%",
  "image_url": "https://elise.vn/media/catalog/product/...",
  "hover_image_url": "https://elise.vn/media/catalog/product/...",
  "product_url": "https://elise.vn/catalog/product/view/id/73418/...",
  "category": "Thời trang nữ",
  "is_sale": true
}
```

### CSV Format:
| ID | Tên sản phẩm | Giá | Giá gốc | % Giảm giá | Hình ảnh | URL | Danh mục | Sale |
|----|--------------|-----|---------|------------|----------|-----|----------|------|

## Lưu ý

- Tool tự động thêm delay 1 giây giữa các request để tránh spam
- Sử dụng User-Agent giả lập trình duyệt
- Xử lý lỗi và timeout
- Hỗ trợ encoding UTF-8 cho tiếng Việt

## Khắc phục sự cố

### Lỗi kết nối:
- Kiểm tra kết nối internet
- Thử lại sau vài phút

### Lỗi thư viện:
```bash
pip install --upgrade requests beautifulsoup4 lxml
```

### Lỗi encoding:
- Đảm bảo terminal hỗ trợ UTF-8
- Trên Windows: `chcp 65001`

## Mở rộng

Tool có thể dễ dàng mở rộng để:
- Cào từ nhiều trang web khác
- Thêm filter theo danh mục
- Cào reviews và đánh giá
- Theo dõi thay đổi giá
- Tích hợp database

## Liên hệ

Nếu có vấn đề hoặc đề xuất, vui lòng tạo issue hoặc liên hệ trực tiếp.