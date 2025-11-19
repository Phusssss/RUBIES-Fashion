using WebBanQuanAo.Models;

namespace WebBanQuanAo.Serveice
{
    public interface IInventoryService
    {
        Task<bool> UpdateStock(int productId, int quantity, string type, string reason, int? userId = null);
        Task<List<InventoryHistory>> GetInventoryHistory(int productId);
        Task<List<Product>> GetLowStockProducts(int threshold = 10);
        Task<bool> CheckStock(int productId, int requiredQuantity);
    }
}