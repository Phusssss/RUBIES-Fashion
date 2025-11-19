using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.IO;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string BankCode = "970422"; // Mã ngân hàng MB Bank theo chuẩn Napas
        private const string AccountNumber = "0395752407"; // Số tài khoản nhận tiền
        private const string BankName = "MB"; // Tên ngân hàng
        private const string CountryCode = "VN"; // Quốc gia
        private const string CurrencyCode = "704"; // Mã tiền tệ (VND)

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1, string selectedSize = null, string selectedColor = null)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            
            var cartItem = new CartItem
            {
                Product = product,
                Quantity = quantity,
                SelectedSize = selectedSize,
                SelectedColor = selectedColor
            };
            
            // Tìm item có cùng sản phẩm, size, màu
            var existingItem = cart.FirstOrDefault(c => c.CartKey == cartItem.CartKey);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(cartItem);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { success = true, message = "Thêm vào giỏ hàng thành công" });
        }
        
        [HttpPost]
        public IActionResult PlaceOrder(string address, string phone, string fullName)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Account");
            }

            var totalAmount = cart.Sum(c => c.Product.Price * c.Quantity);
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Pending",
                Address = address,
                Phone = phone,
                FullName = fullName,
                OrderDetails = cart.Select(c => new OrderDetail
                {
                    ProductId = c.Product.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product.Price,
                    SelectedSize = c.SelectedSize,
                    SelectedColor = c.SelectedColor
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges(); // 🔥 Lưu vào database để order.Id có giá trị

            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("GenerateVietQR", new { amount = totalAmount, orderId = order.OrderId }); // ✅ Giờ order.Id đã có
        }


        public IActionResult GenerateVietQR(decimal amount, int orderId)
        {
            string bankCode = "970418"; // MB Bank
            string accountNumber = "6411112098";
            string description = $"Thanh Toan Don Hang {orderId}";

            string vietQRImageUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact2.png?amount={amount}&addInfo={Uri.EscapeDataString(description)}";

            ViewBag.QRCodeImage = vietQRImageUrl;
            return View();
        }

    }
}
