using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddReview(int productId, int userId, int rating, string comment)
        {
            // Kiểm tra user đã review chưa
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
            
            if (existingReview != null)
                return false; // Đã review rồi

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Review>> GetProductReviews(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRating(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<bool> HasUserReviewed(int productId, int userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.UserId == userId);
        }

        public async Task<int> GetReviewCount(int productId)
        {
            return await _context.Reviews
                .CountAsync(r => r.ProductId == productId);
        }
    }
}