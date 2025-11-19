using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class InventoryHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        [Required]
        public string Type { get; set; } // "IN" hoặc "OUT"
        
        [Required]
        public int Quantity { get; set; }
        
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        
        [Required]
        public string Reason { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}