using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public class ReviewData
    {
        public List<Review> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool HasUserReviewed { get; set; }
    }

    public interface IReviewService
    {
        Task<bool> AddReview(int productId, int userId, int rating, string comment);
        Task<ReviewData> GetProductReviewData(int productId, int? userId = null);
    }
}