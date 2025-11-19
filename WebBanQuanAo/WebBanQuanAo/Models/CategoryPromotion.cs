using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class CategoryPromotion
    {
        public int CategoryPromotionId { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        public int DiscountPercentage { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}