-- Sửa dữ liệu sản phẩm bị lỗi
UPDATE Products 
SET AvailableSizes = 'S,M,L,XL',
    AvailableColors = 'Đen,Trắng,Xanh'
WHERE AvailableSizes = '[]' OR AvailableSizes IS NULL OR AvailableSizes = '';

-- Sửa tên sản phẩm bị lặp lại
UPDATE Products 
SET Name = 'ĐẦM TUYTSY ĐEN CỔ VEST BUỘC DÂY EO',
    Description = 'Đầm tuytsy đen cổ vest buộc dây eo sang trọng'
WHERE ProductId = 1035;

-- Sửa tên sản phẩm ID 1034 nếu cần
UPDATE Products 
SET Name = 'ĐẦM TUYTSY ĐEN CỔ VEST BUỘC DÂY EO'
WHERE ProductId = 1034;