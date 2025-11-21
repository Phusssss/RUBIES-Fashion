using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class ImageProduct
    {
        public int ImageProductId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public string ImageUrl { get; set; }
        
        public bool IsMain { get; set; } = false;
        
        public int DisplayOrder { get; set; } = 0;
        
        public Product Product { get; set; }
    }
}