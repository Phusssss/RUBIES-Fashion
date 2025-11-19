using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Models;
using Microsoft.AspNetCore.Identity;
using WebBanQuanAo.Serveice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace WebBanQuanAo.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AccountController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username đã tồn tại chưa
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại chưa
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(model);
                }

                // Tạo user mới với mật khẩu được mã hóa
                var user = new User
                {
                    Username = model.Username,
                    FullName = model.FullName,
                    Email = model.Email,
                    Role = "Customer" // Role mặc định cho người dùng mới
                };
                
                // Mã hóa mật khẩu
                user.Password = _passwordHasher.HashPassword(user, model.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            return View(model);
        }
    

        // Hiển thị trang đăng nhập
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Tìm user theo username
            var user = _context.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View();
            }

            // Xác thực mật khẩu
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View();
            }

            // Tạo JWT token
            var token = _jwtService.GenerateToken(user);

            // Lưu thông tin người dùng vào session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("JwtToken", token);

            TempData["Success"] = "Đăng nhập thành công!";
            return RedirectToAction("Index", "Home");
        }

        // Đăng nhập bằng Google
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleCallback", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        // Xử lý callback từ Google  
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync();
            
            if (!result.Succeeded)
            {
                TempData["Error"] = "Đăng nhập Google thất bại!";
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Claims;
            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var fullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var picture = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            // Tìm user theo GoogleId hoặc Email
            var user = _context.Users.FirstOrDefault(u => u.GoogleId == googleId || u.Email == email);
            
            if (user == null)
            {
                // Tạo user mới
                user = new User
                {
                    GoogleId = googleId,
                    Email = email,
                    FullName = fullName,
                    Username = email,
                    Role = "Customer",
                    ProfilePicture = picture
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(user.GoogleId))
            {
                // Liên kết tài khoản hiện tại với Google
                user.GoogleId = googleId;
                user.ProfilePicture = picture;
                await _context.SaveChangesAsync();
            }

            // Tạo JWT token
            var token = _jwtService.GenerateToken(user);

            // Lưu thông tin vào session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("JwtToken", token);

            TempData["Success"] = "Đăng nhập Google thành công!";
            return RedirectToAction("Index", "Home");
        }

        // Xử lý đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Login");
        }
    }
}
