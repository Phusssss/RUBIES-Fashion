# 🚀 Hướng dẫn cài đặt Python và chạy Scraper

## ❌ Lỗi: 'pip' is not recognized

Lỗi này có nghĩa là Python chưa được cài đặt hoặc chưa được thêm vào PATH.

## ✅ Giải pháp

### Bước 1: Cài đặt Python

1. **Tải Python**: Truy cập https://www.python.org/downloads/
2. **Tải phiên bản mới nhất** (Python 3.11 hoặc 3.12)
3. **QUAN TRỌNG**: Khi cài đặt, **PHẢI TICK** vào ô "Add Python to PATH"

![Python Installation](https://docs.python.org/3/_images/win_installer.png)

### Bước 2: Kiểm tra cài đặt

Mở Command Prompt mới và chạy:
```cmd
python --version
```

Nếu hiển thị version (ví dụ: Python 3.11.0) thì đã thành công!

### Bước 3: Chạy tool

#### Cách 1: Tự động (Khuyến nghị)
```cmd
install_python.bat
```

#### Cách 2: Thủ công
```cmd
python -m pip install requests beautifulsoup4 lxml
python fashion_scraper.py
```

## 🔧 Khắc phục sự cố

### Vấn đề 1: Python đã cài nhưng vẫn lỗi
**Nguyên nhân**: Python chưa được thêm vào PATH

**Giải pháp**:
1. Gỡ cài đặt Python cũ
2. Cài đặt lại và **NHỚ TICK** "Add Python to PATH"

### Vấn đề 2: Lỗi permission
**Giải pháp**: Chạy Command Prompt với quyền Administrator

### Vấn đề 3: Lỗi SSL/Certificate
**Giải pháp**:
```cmd
python -m pip install --trusted-host pypi.org --trusted-host pypi.python.org --trusted-host files.pythonhosted.org requests beautifulsoup4 lxml
```

## 📋 Checklist cài đặt

- [ ] Tải Python từ python.org
- [ ] Tick "Add Python to PATH" khi cài đặt
- [ ] Restart Command Prompt sau khi cài
- [ ] Chạy `python --version` để kiểm tra
- [ ] Chạy `install_python.bat`
- [ ] Chạy `run_scraper.bat`

## 🎯 Lệnh nhanh

Nếu Python đã cài đặt đúng:
```cmd
cd D:\WebBanQuanAo\WebBanQuanAo\WebBanQuanAo
python -m pip install requests beautifulsoup4 lxml
python fashion_scraper.py
```

## 📞 Hỗ trợ

Nếu vẫn gặp lỗi, hãy:
1. Chụp ảnh màn hình lỗi
2. Cho biết phiên bản Windows
3. Cho biết đã cài Python chưa và phiên bản nào