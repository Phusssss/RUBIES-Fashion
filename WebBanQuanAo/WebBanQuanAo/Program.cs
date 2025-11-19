using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;
using Microsoft.Extensions.DependencyInjection;
using WebBanQuanAo.Serveice;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext với chuỗi kết nối trong appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của Session
    options.Cookie.HttpOnly = true; // Bảo mật cookie
    options.Cookie.IsEssential = true; // Cookie cần thiết
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// Đăng ký dịch vụ bảo mật
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();

// Cấu hình JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSecretKeyHere123456789012345678901234567890";

builder.Services.AddAuthentication("Cookies")
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
    options.SaveTokens = true;
});

// Đăng ký dịch vụ Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Cấu hình middleware cho ứng dụng
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Trang lỗi chi tiết trong chế độ phát triển
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kích hoạt Session trước Authentication
app.UseSession();

// Kích hoạt Authentication trước Authorization
app.UseAuthentication();

// Kích hoạt Authorization
app.UseAuthorization();

// Định tuyến mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed the database with an admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();

    var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");

    if (adminUser != null)
    {
        // Admin user exists, update the password and role if needed
        adminUser.Password = passwordHasher.HashPassword(adminUser, "admin123");
        adminUser.Role = "Admin"; // Ensure the role is Admin
    }
    else
    {
        // Admin user does not exist, create a new one
        adminUser = new User
        {
            Username = "admin",
            FullName = "Admin",
            Email = "admin@example.com",
            Role = "Admin"
        };
        adminUser.Password = passwordHasher.HashPassword(adminUser, "admin123");
        context.Users.Add(adminUser);
    }
    context.SaveChanges();
}

app.Run();
