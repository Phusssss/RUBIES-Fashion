using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class SizeProduct
    {
        public int SizeProductId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [StringLength(10)]
        public string SizeName { get; set; }
        
        public int Stock { get; set; } = 0;
        
        public Product Product { get; set; }
    }
}