using Microsoft.AspNetCore.Mvc;

namespace KomuNect.Controllers;

public class AuthController : Controller
{
    // 1. Role Selection Page
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string Email, string Password, string Role)
    {
        bool isPasswordCorrect = false; 

        if (!isPasswordCorrect)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password. Please try again.");

            ViewBag.Role = Role;
            return View();
        }

        if (Role == "admin")
        {
            return RedirectToAction("Index", "Announcement");
        }
        else
        {
            return RedirectToAction("Index", "Announcement");
        }
    }
}