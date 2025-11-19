using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Models;
using WebBanQuanAo.Attributes;

namespace WebBanQuanAo.Controllers
{
    [AuthorizeRole("Admin")]
    public class ListAccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListAccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display list of users
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

       

        // Display delete user confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // Handle delete user confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
