using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập URL hình ảnh")]
        public bool IsNew { get; set; }
        public bool IsTrend { get; set; }
        public string imgurl { get; set; }

        // Thêm thuộc tính ListImg
        public List<string> ListImg { get; set; }  // Đây là danh sách URL các hình ảnh
        
        // Thêm Size và Color
        public List<string> AvailableSizes { get; set; } = new List<string>(); // S, M, L, XL, XXL
        public List<string> AvailableColors { get; set; } = new List<string>(); // Đỏ, Xanh, Vàng, etc
    }
}
