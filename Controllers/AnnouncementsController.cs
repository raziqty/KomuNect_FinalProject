using KomuNect.Data;
using KomuNect.Models.Entities;
using KomuNect.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KomuNect.Controllers
{
    public class AnnouncementsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AnnouncementsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var announcements = await _dbContext.Announcements
                .Include(a => a.Author)
                .Include(a => a.Category)
                .OrderByDescending(a => a.PostedAt)
                .ToListAsync();
            return View(announcements);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new AddAnnouncementViewModel
            {
                Categories = await GetCategorySelectList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAnnouncementViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await GetCategorySelectList();
                return View(viewModel);
            }

            var authorId = HttpContext.Session.GetInt32("AdminId");
            if (authorId is null) return RedirectToAction("Login", "Admin");

            var announcement = new Announcement
            {
                AuthorId = authorId.Value,
                CategoryId = viewModel.CategoryId,
                Title = viewModel.Title,
                Content = viewModel.Content,
                EventDate = viewModel.EventDate,
                EventLocation = viewModel.EventLocation,
                PostedAt = DateTime.UtcNow
            };

            await _dbContext.Announcements.AddAsync(announcement);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _dbContext.Announcements.FindAsync(id);
            if (announcement is null) return NotFound();

            return View(new EditAnnouncementViewModel
            {
                Id = announcement.Id,
                CategoryId = announcement.CategoryId,
                Title = announcement.Title,
                Content = announcement.Content,
                EventDate = announcement.EventDate,
                EventLocation = announcement.EventLocation,
                Categories = await GetCategorySelectList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAnnouncementViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await GetCategorySelectList();
                return View(viewModel);
            }

            var announcement = await _dbContext.Announcements.FindAsync(viewModel.Id);
            if (announcement is null) return NotFound();

            announcement.CategoryId = viewModel.CategoryId;
            announcement.Title = viewModel.Title;
            announcement.Content = viewModel.Content;
            announcement.EventDate = viewModel.EventDate;
            announcement.EventLocation = viewModel.EventLocation;
            announcement.EditedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var announcement = await _dbContext.Announcements.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (announcement is not null)
            {
                _dbContext.Announcements.Remove(announcement);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectList() =>
            await _dbContext.AnnouncementCategories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName })
                .ToListAsync();
    }
}
