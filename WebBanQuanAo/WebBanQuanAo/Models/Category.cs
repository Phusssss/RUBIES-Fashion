namespace WebBanQuanAo.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        // Navigation property để liên kết với Products
        public ICollection<Product>? Products { get; set; }
    }

}
