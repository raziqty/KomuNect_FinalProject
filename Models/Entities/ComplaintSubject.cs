using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KomuNect.Models.Entities
{
    [Table("complaint_subjects")]
    public class ComplaintSubject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("subject_name")]
        public string SubjectName { get; set; } = string.Empty;

        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
