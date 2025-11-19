using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

 
        public string Email { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; } // Customer, Admin
        
        public string? GoogleId { get; set; }
        
        public string? ProfilePicture { get; set; }
    }
}
