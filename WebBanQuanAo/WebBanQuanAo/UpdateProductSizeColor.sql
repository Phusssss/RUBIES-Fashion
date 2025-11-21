-- Cập nhật size và color mặc định cho các sản phẩm
UPDATE Products 
SET AvailableSizes = 'S,M,L,XL'
WHERE AvailableSizes IS NULL OR AvailableSizes = '';

UPDATE Products 
SET AvailableColors = 'Đen,Trắng,Xanh'
WHERE AvailableColors IS NULL OR AvailableColors = '';