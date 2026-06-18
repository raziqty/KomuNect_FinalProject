using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KomuNect_Demo.Models.ViewModels
{
    public class AddAnnouncementViewModel
    {
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date")]
        public DateTime? EventDate { get; set; }

        [MaxLength(150)]
        [Display(Name = "Event Location")]
        public string? EventLocation { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }

    public class EditAnnouncementViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date")]
        public DateTime? EventDate { get; set; }

        [MaxLength(150)]
        [Display(Name = "Event Location")]
        public string? EventLocation { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}