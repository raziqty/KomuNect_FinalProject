using KomuNect_Demo.Data;
using KomuNect_Demo.Models.Entities;
using KomuNect_Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KomuNect_Demo.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ResidentsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Public registration — any visitor can access this
        [HttpGet]
        public IActionResult Register() => View(new AddResidentViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(AddResidentViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            // Prevent duplicate email
            var exists = await _dbContext.Residents.AnyAsync(r => r.Email == viewModel.Email);
            if (exists)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(viewModel);
            }

            var resident = new Resident
            {
                FirstName = viewModel.FirstName,
                MiddleName = viewModel.MiddleName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                Address = viewModel.Address,
                Birthdate = viewModel.Birthdate,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Residents.AddAsync(resident);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View(new ResidentLoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(ResidentLoginViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var resident = await _dbContext.Residents.FirstOrDefaultAsync(r => r.Email == viewModel.Email);
            if (resident is null || !BCrypt.Net.BCrypt.Verify(viewModel.Password, resident.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(viewModel);
            }

            HttpContext.Session.SetInt32("ResidentId", resident.Id);
            HttpContext.Session.SetString("ResidentName", $"{resident.FirstName} {resident.LastName}");
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