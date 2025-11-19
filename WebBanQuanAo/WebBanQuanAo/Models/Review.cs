using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5 sao")]
        public int Rating { get; set; }
        
        [Required]
        [StringLength(500, ErrorMessage = "Bình luận không được quá 500 ký tự")]
        public string Comment { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}