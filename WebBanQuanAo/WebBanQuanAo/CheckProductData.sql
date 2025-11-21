-- Kiểm tra dữ liệu sản phẩm
SELECT ProductId, Name, Description, AvailableSizes, AvailableColors 
FROM Products 
WHERE Name LIKE '%ĐẦM TUYTSY%';

-- Cập nhật dữ liệu sản phẩm bị lỗi (nếu cần)
-- UPDATE Products 
-- SET Name = 'ĐẦM TUYTSY ĐEN CỔ VEST BUỘC DÂY EO',
--     Description = 'Đầm tuytsy đen cổ vest buộc dây eo sang trọng'
-- WHERE ProductId = [ID_CỦA_SẢN_PHẨM_BỊ_LỖI];