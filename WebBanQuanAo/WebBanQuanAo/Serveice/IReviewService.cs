using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public interface IReviewService
    {
        Task<bool> AddReview(int productId, int userId, int rating, string comment);
        Task<List<Review>> GetProductReviews(int productId);
        Task<double> GetAverageRating(int productId);
        Task<bool> HasUserReviewed(int productId, int userId);
        Task<int> GetReviewCount(int productId);
    }
}