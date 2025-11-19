using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Serveice;

namespace WebBanQuanAo.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để đánh giá!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var success = await _reviewService.AddReview(productId, userId.Value, rating, comment);
            
            if (success)
                TempData["Success"] = "Đánh giá thành công!";
            else
                TempData["Error"] = "Bạn đã đánh giá sản phẩm này rồi!";

            return RedirectToAction("Details", "Product", new { id = productId });
        }

        public async Task<IActionResult> GetReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviews(productId);
            return PartialView("_ReviewList", reviews);
        }
    }
}