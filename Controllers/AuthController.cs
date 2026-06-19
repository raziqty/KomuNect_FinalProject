using BCrypt.Net;
using KomuNect.Data; // Ensure this points to where your ApplicationDbContext is
using KomuNect.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string LoginIdentifier, string Password)
        {
            if (string.IsNullOrEmpty(LoginIdentifier) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Please enter your login details.");
                return View();
            }

            Admin adminUser = null;
            if (int.TryParse(LoginIdentifier, out int parsedId))
            {
                adminUser = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == parsedId || a.Username == LoginIdentifier);
            }
            else
            {
                adminUser = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Username == LoginIdentifier);
            }

            if (adminUser != null && BCrypt.Net.BCrypt.Verify(Password, adminUser.PasswordHash))
            {
                HttpContext.Session.SetInt32("AdminId", adminUser.Id);
                return RedirectToAction("List", "Announcements");
            }

            var residentUser = await _dbContext.Residents.FirstOrDefaultAsync(r => r.Email == LoginIdentifier);

            if (residentUser != null && BCrypt.Net.BCrypt.Verify(Password, residentUser.PasswordHash))
            {
                HttpContext.Session.SetInt32("ResidentId", residentUser.Id);
                return RedirectToAction("List", "Announcements");
            }

            ModelState.AddModelError(string.Empty, "Invalid login credentials.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(string FirstName, string? MiddleName, string LastName, string Email, string Password, string ConfirmPassword, string Address, DateTime Birthdate)
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Please fill in all required fields.");
                ViewBag.Role = "resident";
                return View();
            }

            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match. Please try again.");
                ViewBag.Role = "resident";
                return View();
            }

            var existingResident = await _dbContext.Residents.FirstOrDefaultAsync(r => r.Email == Email);
            if (existingResident != null)
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists! Try logging in.");
                ViewBag.Role = "resident";
                return View();
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var newResident = new Resident
            {
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Email = Email,
                PasswordHash = hashedPassword,
                Address = Address ?? "No address provided",
                Birthdate = DateOnly.FromDateTime(Birthdate),
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Residents.Add(newResident);
            await _dbContext.SaveChangesAsync();

            HttpContext.Session.SetInt32("ResidentId", newResident.Id);

            return RedirectToAction("List", "Announcements");
        }
    }
}