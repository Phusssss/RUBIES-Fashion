# Hướng dẫn thiết lập Google OAuth

## Bước 1: Tạo Google Cloud Project

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Kích hoạt Google+ API

## Bước 2: Tạo OAuth 2.0 Credentials

1. Vào **APIs & Services** > **Credentials**
2. Click **Create Credentials** > **OAuth client ID**
3. Chọn **Web application**
4. Thêm **Authorized redirect URIs**:
   - `https://localhost:7150/Account/GoogleCallback`
   - `http://localhost:5150/Account/GoogleCallback`
   - `https://yourdomain.com/Account/GoogleCallback` (cho production)

## Bước 3: Cập nhật appsettings.json

Thay thế `YOUR_GOOGLE_CLIENT_ID` và `YOUR_GOOGLE_CLIENT_SECRET` trong file `appsettings.json`:

```json
"GoogleAuth": {
    "ClientId": "your-actual-client-id.googleusercontent.com",
    "ClientSecret": "your-actual-client-secret"
}
```

## Bước 4: Chạy ứng dụng

```bash
dotnet run
```

## Tính năng đã thêm:

1. **Đăng nhập Google**: Customer có thể đăng nhập bằng tài khoản Google
2. **Tự động tạo tài khoản**: Nếu chưa có tài khoản, hệ thống sẽ tự động tạo
3. **Liên kết tài khoản**: Nếu đã có tài khoản với email trùng, sẽ liên kết với Google
4. **Lưu ảnh đại diện**: Ảnh profile từ Google sẽ được lưu vào database

## Lưu ý bảo mật:

- Không commit Client Secret vào Git
- Sử dụng User Secrets cho development
- Sử dụng Environment Variables cho production