using Microsoft.EntityFrameworkCore;
using KomuNect_Demo.Models.Entities;

namespace KomuNect_Demo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<AnnouncementCategory> AnnouncementCategories { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ComplaintSubject> ComplaintSubjects { get; set; }
        public DbSet<Complaint> Complaints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>().HasIndex(a => a.AdminId).IsUnique();
            modelBuilder.Entity<Admin>().HasIndex(a => a.Username).IsUnique();
            modelBuilder.Entity<Resident>().HasIndex(r => r.Email).IsUnique();
            modelBuilder.Entity<AnnouncementCategory>().HasIndex(ac => ac.CategoryName).IsUnique();
            modelBuilder.Entity<ComplaintSubject>().HasIndex(cs => cs.SubjectName).IsUnique();

            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.Author)
                .WithMany(ad => ad.Announcements)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.Category)
                .WithMany(ac => ac.Announcements)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Resident)
                .WithMany(r => r.Complaints)
                .HasForeignKey(c => c.ResidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Subject)
                .WithMany(cs => cs.Complaints)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<AnnouncementCategory>().HasData(
                new AnnouncementCategory { Id = 1, CategoryName = "General" },
                new AnnouncementCategory { Id = 2, CategoryName = "Health" },
                new AnnouncementCategory { Id = 3, CategoryName = "Safety" },
                new AnnouncementCategory { Id = 4, CategoryName = "Events" },
                new AnnouncementCategory { Id = 5, CategoryName = "Infrastructure" }
            );

            modelBuilder.Entity<ComplaintSubject>().HasData(
                new ComplaintSubject { Id = 1, SubjectName = "Noise Complaint" },
                new ComplaintSubject { Id = 2, SubjectName = "Garbage / Sanitation" },
                new ComplaintSubject { Id = 3, SubjectName = "Illegal Structures" },
                new ComplaintSubject { Id = 4, SubjectName = "Public Disturbance" },
                new ComplaintSubject { Id = 5, SubjectName = "Others" }
            );
        }

        public void SeedAdmins()
        {
            var admins = new[]
            {
                new { AdminId = "ad-0231", Username = "juandelacruz", Password = "wordpass333" },
            };

            foreach (var a in admins)
            {
                if (!Admins.Any(x => x.Username == a.Username))
                {
                    Admins.Add(new Admin
                    {
                        AdminId = a.AdminId,
                        Username = a.Username,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(a.Password),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            SaveChanges();
        }
    }
}
