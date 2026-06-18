using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using KomuNect.Data; // Ensure this points to where your ApplicationDbContext is
using BCrypt.Net;

namespace KomuNect.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        // 1. Inject the Database into the Controller
        public AuthController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult RoleSelection(string mode = "login")
        {
            ViewBag.Mode = mode;
            return View();
        }

        [HttpGet]
        public IActionResult Login(string role = "resident")
        {
            ViewBag.Role = role;
            return View();
        }

        [HttpGet]
        public IActionResult Signup(string role = "resident")
        {
            ViewBag.Role = role;
            return View();
        }

        // 2. Renamed 'Email' to 'Identifier' so it can accept either an Email OR an Admin ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Identifier, string Password, string Role)
        {
            if (Role == "admin")
            {
                // ADMIN LOGIN: Look up by AdminId instead of Email
                var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.AdminId == Identifier);

                // Verify the hashed password
                if (admin != null && BCrypt.Net.BCrypt.Verify(Password, admin.PasswordHash))
                {
                    HttpContext.Session.SetInt32("AdminId", admin.Id);
                    return RedirectToAction("List", "Announcements");
                }
            }
            else
            {
                // RESIDENT LOGIN: Look up by Email
                var resident = await _dbContext.Residents.FirstOrDefaultAsync(r => r.Email == Identifier);

                // Verify the hashed password
                if (resident != null && BCrypt.Net.BCrypt.Verify(Password, resident.PasswordHash))
                {
                    HttpContext.Session.SetInt32("ResidentId", resident.Id);
                    return RedirectToAction("List", "Complaints");
                }
            }

            // If we get here, the login failed
            ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
            ViewBag.Role = Role;
            return View();
        }
    }
}