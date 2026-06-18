using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomuNect.Models.Entities
{
    [Table("announcement_categories")]
    public class AnnouncementCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("category_name")]
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    }
}
