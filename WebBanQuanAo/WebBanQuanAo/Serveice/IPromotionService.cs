using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public interface IPromotionService
    {
        Task<Coupon> ValidateCoupon(string code, decimal orderAmount);
        Task<decimal> ApplyCoupon(string code, decimal orderAmount);
        Task<List<FlashSale>> GetActiveFlashSales();
        Task<FlashSale> GetFlashSaleByProduct(int productId);
        Task<decimal> GetFlashSalePrice(int productId);
        Task<CategoryPromotion> GetCategoryPromotion(int categoryId);
        Task<decimal> ApplyCategoryDiscount(int categoryId, decimal originalPrice);
    }
}