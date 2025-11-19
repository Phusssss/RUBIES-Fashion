using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class FlashSale
    {
        public int FlashSaleId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        [Required]
        public decimal OriginalPrice { get; set; }
        
        [Required]
        public decimal SalePrice { get; set; }
        
        public int DiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; } = 0;
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}