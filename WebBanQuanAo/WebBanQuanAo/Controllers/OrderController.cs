using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;
using X.PagedList;
using X.PagedList.Extensions;
using OfficeOpenXml; // Add this using directive
using System.IO; // Add this using directive
using WebBanQuanAo.Serveice;
namespace WebBanQuanAo.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;

        public OrderController(ApplicationDbContext context, IInventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        // Kiểm tra quyền truy cập (chỉ cho Admin)
        private bool IsAdmin()
        {
        var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }
        // Add this method to your OrderController
        public async Task<IActionResult> ExportToExcel(string searchOrderId, string statusFilter, DateTime? startDate, DateTime? endDate)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            var ordersQuery = _context.Orders.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(searchOrderId) && int.TryParse(searchOrderId, out int orderId))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderId == orderId);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == statusFilter);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);
            }

            var orders = await ordersQuery.ToListAsync();

            // Thiết lập LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Người đặt";
                worksheet.Cells[1, 3].Value = "Ngày đặt";
                worksheet.Cells[1, 4].Value = "Tổng tiền";
                worksheet.Cells[1, 5].Value = "Trạng thái";

                for (int i = 0; i < orders.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = orders[i].OrderId;
                    worksheet.Cells[i + 2, 2].Value = orders[i].User?.FullName;
                    worksheet.Cells[i + 2, 3].Value = orders[i].OrderDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 4].Value = orders[i].TotalAmount.ToString("C");
                    worksheet.Cells[i + 2, 5].Value = orders[i].Status;
                }

                worksheet.Cells.AutoFitColumns();

                // Trả về file Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileName = $"Orders_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        // Danh sách đơn hàng
        public async Task<IActionResult> Index(string searchOrderId, string statusFilter, DateTime? startDate, DateTime? endDate, int? page)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            var ordersQuery = _context.Orders.Include(o => o.User).AsQueryable();

            // Tìm kiếm theo mã đơn hàng
            if (!string.IsNullOrEmpty(searchOrderId) && int.TryParse(searchOrderId, out int orderId))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderId == orderId);
            }

            // Lọc theo trạng thái đơn hàng
            if (!string.IsNullOrEmpty(statusFilter))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == statusFilter);
            }

            // Lọc theo thời gian
            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);
            }

            int pageSize = 5; // Số đơn hàng trên mỗi trang
            int pageNumber = (page ?? 1);

            var orders = ordersQuery.OrderByDescending(o => o.OrderDate).ToPagedList(pageNumber, pageSize);
            ViewData["SearchOrderId"] = searchOrderId;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền thực hiện thao tác này.";
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            
            if (order == null)
            {
                return NotFound();
            }

            // Nếu đơn hàng được xác nhận, trừ kho
            if (newStatus == "Confirmed" && order.Status != "Confirmed")
            {
                foreach (var detail in order.OrderDetails)
                {
                    await _inventoryService.UpdateStock(
                        detail.ProductId, 
                        detail.Quantity, 
                        "OUT", 
                        $"Bán hàng - Đơn #{orderId}",
                        HttpContext.Session.GetInt32("UserId")
                    );
                }
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
