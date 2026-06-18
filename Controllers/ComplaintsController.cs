using KomuNect.Data;
using KomuNect.Models.Entities;
using KomuNect.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KomuNect.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ComplaintsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            var residentId = HttpContext.Session.GetInt32("ResidentId");

            IQueryable<Complaint> query = _dbContext.Complaints
                .Include(c => c.Resident)
                .Include(c => c.Subject)
                .OrderByDescending(c => c.FiledAt);

            if (adminId is null && residentId is not null)
                query = query.Where(c => c.ResidentId == residentId.Value);

            return View(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new AddComplaintViewModel
            {
                Subjects = await GetSubjectSelectList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddComplaintViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Subjects = await GetSubjectSelectList();
                return View(viewModel);
            }

            var residentId = HttpContext.Session.GetInt32("ResidentId");
            if (residentId is null) return RedirectToAction("Login", "Residents");

            var complaint = new Complaint
            {
                ResidentId = residentId.Value,
                SubjectId = viewModel.SubjectId,
                Details = viewModel.Details,
                Status = ComplaintStatus.Pending,
                FiledAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Complaints.AddAsync(complaint);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var complaint = await _dbContext.Complaints
                .Include(c => c.Resident)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint is null) return NotFound();

            return View(new EditComplaintViewModel
            {
                Id = complaint.Id,
                Status = complaint.Status,
                AdminNote = complaint.AdminNote
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditComplaintViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var complaint = await _dbContext.Complaints.FindAsync(viewModel.Id);
            if (complaint is null) return NotFound();

            complaint.Status = viewModel.Status;
            complaint.AdminNote = viewModel.AdminNote;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var complaint = await _dbContext.Complaints.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (complaint is not null)
            {
                _dbContext.Complaints.Remove(complaint);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }

        private async Task<IEnumerable<SelectListItem>> GetSubjectSelectList() =>
            await _dbContext.ComplaintSubjects
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.SubjectName })
                .ToListAsync();
    }
}
