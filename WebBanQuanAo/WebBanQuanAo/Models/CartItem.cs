using Newtonsoft.Json;

namespace WebBanQuanAo.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public string? SelectedSize { get; set; }
        public string? SelectedColor { get; set; }
        
        [JsonIgnore]
        public Product? Product { get; set; }
        
        public string CartKey => $"{ProductId}_{SelectedSize}_{SelectedColor}";
    }
}
