using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class ColorProduct
    {
        public int ColorProductId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ColorName { get; set; }
        
        [StringLength(7)]
        public string ColorCode { get; set; } // Hex color code
        
        public Product Product { get; set; }
    }
}