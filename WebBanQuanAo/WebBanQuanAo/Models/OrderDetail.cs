namespace WebBanQuanAo.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } // Liên kết với Order
        public int ProductId { get; set; }
        public Product Product { get; set; } // Liên kết với Product
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Giá tại thời điểm mua
        public string? SelectedSize { get; set; } // Size được chọn
        public string? SelectedColor { get; set; } // Màu được chọn
    }

}
