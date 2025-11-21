# Script để xóa và tạo lại dữ liệu mẫu
$baseUrl = "https://localhost:7150"

# Bỏ qua SSL certificate validation
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "Đang xóa toàn bộ dữ liệu sản phẩm..." -ForegroundColor Yellow
try {
    $response1 = Invoke-RestMethod -Uri "$baseUrl/DataSeed/ClearAllProducts" -Method POST
    Write-Host "✓ Đã xóa toàn bộ dữ liệu sản phẩm" -ForegroundColor Green
} catch {
    Write-Host "✗ Lỗi khi xóa dữ liệu: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Đang tạo dữ liệu mẫu mới..." -ForegroundColor Yellow
try {
    $response2 = Invoke-RestMethod -Uri "$baseUrl/DataSeed/CreateSampleData" -Method POST
    Write-Host "✓ Đã tạo dữ liệu mẫu thành công" -ForegroundColor Green
} catch {
    Write-Host "✗ Lỗi khi tạo dữ liệu mẫu: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Hoàn thành!" -ForegroundColor Cyan