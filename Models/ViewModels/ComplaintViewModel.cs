using KomuNect.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KomuNect.Models.ViewModels
{
    public class AddComplaintViewModel
    {
        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Details")]
        public string Details { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> Subjects { get; set; } = new List<SelectListItem>();
    }

    public class EditComplaintViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ComplaintStatus Status { get; set; }

        [Display(Name = "Admin Note")]
        public string? AdminNote { get; set; }
    }
}