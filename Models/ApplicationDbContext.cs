using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rezk_Proj.Models
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            // Relationships
            modelbuilder.Entity<Employer>()
                .HasMany(e => e.Jobs)
                .WithOne(j => j.Employer)
                .HasForeignKey(j => j.EmployerId);

            modelbuilder.Entity<Applications>()
                .HasKey(a => new { a.JobId, a.ApplicantId });

            modelbuilder.Entity<Applications>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            modelbuilder.Entity<Applications>()
                .HasOne(a => a.Applicant)
                .WithMany(ap => ap.Applications)
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelbuilder.Entity<Job>()
                .Property(j => j.CreatedAt)
                .HasDefaultValueSql("getdate()");

            modelbuilder.Entity<Applications>()
                .Property(a => a.AppliedAt)
                .HasDefaultValueSql("getdate()");

            modelbuilder.Entity<Employer>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employer>(e => e.UserId);

            modelbuilder.Entity<Applicant>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Applicant>(e => e.UserId);

            modelbuilder.Entity<Category>()
                .HasMany(c => c.Jobs)
                .WithOne(j => j.Category)
                .HasForeignKey(j => j.CategoryId);

            // ---- Fix decimal precision/scale for geo coordinates ----
            // Use precision (total digits, digits after decimal). decimal(9,6) is common for lat/lng.
            modelbuilder.Entity<Applicant>().Property(a => a.Latitude).HasPrecision(18, 2);
            modelbuilder.Entity<Applicant>().Property(a => a.Longitude).HasPrecision(18, 2);

            modelbuilder.Entity<Employer>().Property(e => e.Latitude).HasPrecision(18, 2);
            modelbuilder.Entity<Employer>().Property(e => e.Longitude).HasPrecision(18, 2);

            modelbuilder.Entity<Job>().Property(j => j.Latitude).HasPrecision(18, 2);
            modelbuilder.Entity<Job>().Property(j => j.Longitude).HasPrecision(18, 2);

            // (Optional) If your properties are nullable decimals, you can use typeof(decimal?) overload:
            // modelbuilder.Entity<Applicant>().Property(typeof(decimal), "Latitude").HasPrecision(9,6);
        }

        public DbSet<Employer> Employers { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Category> Categories { get; set; }
        }
}
