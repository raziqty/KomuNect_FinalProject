using KomuNect.Data;
using KomuNect.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomuNect.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login() => View(new AdminLoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Username == viewModel.Username);
            if (admin is null || !BCrypt.Net.BCrypt.Verify(viewModel.Password, admin.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(viewModel);
            }

            HttpContext.Session.SetInt32("AdminId", admin.Id);
            HttpContext.Session.SetString("AdminUsername", admin.Username);
            return RedirectToAction("List", "Announcements");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}