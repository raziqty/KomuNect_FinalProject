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
            // This physically destroys the AdminId and ResidentId from the server memory!
            HttpContext.Session.Clear();

            // Now that they have no memory, send them back to the start
            return RedirectToAction("Index", "Home");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Added MiddleName to the parameters here!
        public async Task<IActionResult> Signup(string FirstName, string? MiddleName, string LastName, string Email, string Password, string Address, DateTime Birthdate)
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError(string.Empty, "Please fill in all required fields.");
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
                MiddleName = MiddleName, // Now it correctly grabs the middle name!
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

            return RedirectToAction("List", "Complaints");
        }
    }
}