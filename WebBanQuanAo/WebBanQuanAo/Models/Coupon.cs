using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        
        public decimal DiscountAmount { get; set; }
        public int DiscountPercentage { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}