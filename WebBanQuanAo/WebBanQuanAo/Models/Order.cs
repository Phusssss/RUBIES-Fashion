namespace WebBanQuanAo.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } // Liên kết với User
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; } // Trạng thái: Pending, Confirmed, Shipped, Completed, Cancelled
        public ICollection<OrderDetail> OrderDetails { get; set; } // Liên kết với OrderDetails
    }

}
