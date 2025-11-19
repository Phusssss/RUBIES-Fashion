# RUBIES Fashion - Website Bán Quần Áo

## Mô tả dự án
Website bán quần áo trực tuyến được xây dựng bằng ASP.NET Core MVC với các tính năng hiện đại và giao diện thân thiện.

## Tính năng chính

### 🛍️ **Khách hàng (Customer)**
- **Đăng ký/Đăng nhập**: Hỗ trợ đăng nhập bằng tài khoản thường và Google OAuth
- **Tìm kiếm nâng cao**: Lọc theo giá, size, màu sắc, danh mục
- **Giỏ hàng thông minh**: Chọn size/màu khi thêm sản phẩm
- **Đặt hàng**: Thanh toán qua VietQR
- **Đánh giá sản phẩm**: Rating 5 sao và bình luận
- **Gợi ý sản phẩm**: Sản phẩm liên quan

### 👨‍💼 **Quản trị viên (Admin)**
- **Quản lý sản phẩm**: CRUD với size/màu, hình ảnh
- **Quản lý đơn hàng**: Xem, cập nhật trạng thái, xuất Excel
- **Quản lý danh mục**: Phân loại sản phẩm
- **Quản lý tài khoản**: Quản lý người dùng
- **Quản lý kho**: Theo dõi tồn kho, lịch sử nhập/xuất
- **Quản lý khuyến mãi**: Mã giảm giá, flash sale

## Công nghệ sử dụng

### Backend
- **ASP.NET Core 8.0 MVC**
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Authentication** - Bảo mật
- **Google OAuth 2.0** - Đăng nhập Google

### Frontend
- **Bootstrap 5** - UI Framework
- **JavaScript/jQuery** - Tương tác
- **Font Awesome** - Icons
- **CSS3** - Styling

### Packages
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.AspNetCore.Authentication.Google`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `EPPlus` - Xuất Excel
- `QRCoder` - Tạo QR Code
- `X.PagedList` - Phân trang

## Cài đặt và chạy dự án

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server)
- Visual Studio 2022 hoặc VS Code

### Bước 1: Clone repository
```bash
git clone https://github.com/yourusername/WebBanQuanAo.git
cd WebBanQuanAo
```

### Bước 2: Cấu hình Database
1. Cập nhật connection string trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebBanQuanAo;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. Chạy migration:
```bash
cd WebBanQuanAo
dotnet ef database update
```

### Bước 3: Cấu hình Google OAuth (Tùy chọn)
1. Tạo project tại [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo OAuth 2.0 credentials
3. Cập nhật `appsettings.json`:
```json
{
  "GoogleAuth": {
    "ClientId": "your-client-id.googleusercontent.com",
    "ClientSecret": "your-client-secret"
  }
}
```

### Bước 4: Chạy ứng dụng
```bash
dotnet run
```

Truy cập: `https://localhost:7150`

## Tài khoản mặc định
- **Admin**: 
  - Username: `admin`
  - Password: `admin123`

## Cấu trúc dự án
```
WebBanQuanAo/
├── Controllers/          # MVC Controllers
├── Models/              # Data Models
├── Views/               # Razor Views
├── Services/            # Business Logic
├── Migrations/          # EF Migrations
├── wwwroot/            # Static files
└── appsettings.json    # Configuration
```

## Tính năng nổi bật

### 🔍 **Tìm kiếm thông minh**
- Full-text search
- Lọc đa tiêu chí
- Autocomplete
- Sắp xếp linh hoạt

### 🛒 **Giỏ hàng nâng cao**
- Chọn size/màu cho từng sản phẩm
- Phân biệt items cùng sản phẩm khác thuộc tính
- Session-based cart

### 💳 **Thanh toán VietQR**
- Tích hợp VietQR API
- Tự động tạo QR code
- Thông tin đơn hàng trong QR

### 📊 **Dashboard Admin**
- Thống kê trực quan
- Quản lý đa dạng
- Xuất báo cáo Excel

## Screenshots

### Trang chủ
![Homepage](screenshots/homepage.png)

### Trang sản phẩm
![Product Details](screenshots/product-details.png)

### Admin Dashboard
![Admin Dashboard](screenshots/admin-dashboard.png)

## Đóng góp
1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## License
Distributed under the MIT License. See `LICENSE` for more information.

## Liên hệ
- **Developer**: Your Name
- **Email**: your.email@example.com
- **Project Link**: [https://github.com/yourusername/WebBanQuanAo](https://github.com/yourusername/WebBanQuanAo)

## Acknowledgments
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Bootstrap](https://getbootstrap.com/)
- [Font Awesome](https://fontawesome.com/)
- [VietQR](https://vietqr.io/)