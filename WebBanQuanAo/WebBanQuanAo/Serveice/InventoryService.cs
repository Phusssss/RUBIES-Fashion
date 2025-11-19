using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateStock(int productId, int quantity, string type, string reason, int? userId = null)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            var stockBefore = product.Stock;
            
            if (type == "OUT" && product.Stock < quantity)
                return false; // Không đủ hàng

            product.Stock = type == "IN" ? product.Stock + quantity : product.Stock - quantity;
            
            var history = new InventoryHistory
            {
                ProductId = productId,
                Type = type,
                Quantity = quantity,
                StockBefore = stockBefore,
                StockAfter = product.Stock,
                Reason = reason,
                UserId = userId
            };

            _context.InventoryHistories.Add(history);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<InventoryHistory>> GetInventoryHistory(int productId)
        {
            return await _context.InventoryHistories
                .Where(h => h.ProductId == productId)
                .Include(h => h.User)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetLowStockProducts(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.Stock <= threshold)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<bool> CheckStock(int productId, int requiredQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.Stock >= requiredQuantity;
        }
    }
}