using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;

        public PromotionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Coupon> ValidateCoupon(string code, decimal orderAmount)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code && 
                                         c.IsActive && 
                                         c.StartDate <= DateTime.Now && 
                                         c.EndDate >= DateTime.Now &&
                                         c.UsedCount < c.UsageLimit &&
                                         orderAmount >= c.MinOrderAmount);
            return coupon;
        }

        public async Task<decimal> ApplyCoupon(string code, decimal orderAmount)
        {
            var coupon = await ValidateCoupon(code, orderAmount);
            if (coupon == null) return 0;

            decimal discount = 0;
            if (coupon.DiscountAmount > 0)
                discount = coupon.DiscountAmount;
            else if (coupon.DiscountPercentage > 0)
                discount = orderAmount * coupon.DiscountPercentage / 100;

            return Math.Min(discount, orderAmount);
        }

        public async Task<List<FlashSale>> GetActiveFlashSales()
        {
            return await _context.FlashSales
                .Include(f => f.Product)
                .Where(f => f.IsActive && 
                           f.StartTime <= DateTime.Now && 
                           f.EndTime >= DateTime.Now &&
                           f.SoldQuantity < f.Quantity)
                .ToListAsync();
        }

        public async Task<FlashSale> GetFlashSaleByProduct(int productId)
        {
            return await _context.FlashSales
                .FirstOrDefaultAsync(f => f.ProductId == productId && 
                                         f.IsActive && 
                                         f.StartTime <= DateTime.Now && 
                                         f.EndTime >= DateTime.Now &&
                                         f.SoldQuantity < f.Quantity);
        }

        public async Task<decimal> GetFlashSalePrice(int productId)
        {
            var flashSale = await GetFlashSaleByProduct(productId);
            return flashSale?.SalePrice ?? 0;
        }

        public async Task<CategoryPromotion> GetCategoryPromotion(int categoryId)
        {
            return await _context.CategoryPromotions
                .FirstOrDefaultAsync(cp => cp.CategoryId == categoryId && 
                                          cp.IsActive && 
                                          cp.StartDate <= DateTime.Now && 
                                          cp.EndDate >= DateTime.Now);
        }

        public async Task<decimal> ApplyCategoryDiscount(int categoryId, decimal originalPrice)
        {
            var promotion = await GetCategoryPromotion(categoryId);
            if (promotion == null) return originalPrice;

            var discount = originalPrice * promotion.DiscountPercentage / 100;
            if (promotion.MaxDiscountAmount > 0)
                discount = Math.Min(discount, promotion.MaxDiscountAmount);

            return originalPrice - discount;
        }
    }
}