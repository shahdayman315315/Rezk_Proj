using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rezk_Proj.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            // Employer -> Jobs
            modelbuilder.Entity<Employer>()
                .HasMany(e => e.Jobs)
                .WithOne(j => j.Employer)
                .HasForeignKey(j => j.EmployerId);

            // Applications composite key
            modelbuilder.Entity<Applications>()
                .HasKey(a => new { a.JobId, a.ApplicantId });

            // Application -> Job (Restrict delete to avoid multiple cascade paths)
            modelbuilder.Entity<Applications>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Application -> Applicant (Cascade delete enabled)
            modelbuilder.Entity<Applications>()
                .HasOne(a => a.Applicant)
                .WithMany(ap => ap.Applications)
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job defaults
            modelbuilder.Entity<Job>()
                .Property(j => j.CreatedAt)
                .HasDefaultValueSql("getdate()");

            // Application defaults
            modelbuilder.Entity<Applications>()
                .Property(a => a.AppliedAt)
                .HasDefaultValueSql("getdate()");

            // Employer -> User
            modelbuilder.Entity<Employer>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employer>(e => e.UserId);

            // Applicant -> User
            modelbuilder.Entity<Applicant>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Applicant>(e => e.UserId);

            // Category -> Jobs
            modelbuilder.Entity<Category>()
                .HasMany(c => c.Jobs)
                .WithOne(j => j.Category)
                .HasForeignKey(j => j.CategoryId);

            // Precision for geo coordinates
            modelbuilder.Entity<Applicant>().Property(a => a.Latitude).HasPrecision(18, 2);
            modelbuilder.Entity<Applicant>().Property(a => a.Longitude).HasPrecision(18, 2);

            modelbuilder.Entity<Employer>().Property(e => e.Latitude).HasPrecision(18, 2);
            modelbuilder.Entity<Employer>().Property(e => e.Longitude).HasPrecision(18, 2);

            modelbuilder.Entity<Job>().Property(j => j.Latitude).HasPrecision(18, 2);
            modelbuilder.Entity<Job>().Property(j => j.Longitude).HasPrecision(18, 2);
        }

        public DbSet<Employer> Employers { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Category> Categories { get; set; }


        public DbSet<Status> StatusTypes { get; set; }

        public DbSet<WorkType> WorkTypeLabels { get; set; }
    }
}
