using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomuNect.Models.Entities
{
    public enum ComplaintStatus { Pending, Action, Resolved }

    [Table("complaints")]
    public class Complaint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("resident_id")]
        public int ResidentId { get; set; }

        [Required]
        [Column("subject_id")]
        public int SubjectId { get; set; }

        [Required]
        [Column("details")]
        public string Details { get; set; } = string.Empty;

        [Required]
        [Column("status")]
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

        [Column("filed_at")]
        public DateTime FiledAt { get; set; } = DateTime.UtcNow;

        [Column("admin_note")]
        public string? AdminNote { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ResidentId")]
        public Resident Resident { get; set; } = null!;

        [ForeignKey("SubjectId")]
        public ComplaintSubject Subject { get; set; } = null!;
    }
}
