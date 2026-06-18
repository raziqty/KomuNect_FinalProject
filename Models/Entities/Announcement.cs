using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomuNect.Models.Entities
{
    [Table("announcements")]
    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("author_id")]
        public int AuthorId { get; set; }

        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("event_date")]
        public DateTime? EventDate { get; set; }

        [MaxLength(150)]
        [Column("event_location")]
        public string? EventLocation { get; set; }

        [Column("posted_at")]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        [Column("edited_at")]
        public DateTime? EditedAt { get; set; }

        [ForeignKey("AuthorId")]
        public Admin Author { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public AnnouncementCategory Category { get; set; } = null!;
    }
}
