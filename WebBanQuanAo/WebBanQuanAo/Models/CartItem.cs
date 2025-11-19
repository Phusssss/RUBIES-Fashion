namespace WebBanQuanAo.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!; // Sản phẩm
        public int Quantity { get; set; } // Số lượng
        public string? SelectedSize { get; set; } // Size được chọn
        public string? SelectedColor { get; set; } // Màu được chọn
        
        // Key để phân biệt các item có cùng sản phẩm nhưng khác size/màu
        public string CartKey => $"{Product.ProductId}_{SelectedSize}_{SelectedColor}";
    }
}
