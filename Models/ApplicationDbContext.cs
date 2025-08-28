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
            modelbuilder.Entity<Employer>().HasMany(e => e.Jobs).WithOne(j => j.Employer).HasForeignKey(j => j.EmployerId);
            modelbuilder.Entity<Applications>().HasKey(a => new { a.JobId, a.ApplicantId });
            modelbuilder.Entity<Applications>().HasOne(a => a.Job).WithMany(j => j.Applications).HasForeignKey(a => a.JobId).OnDelete(DeleteBehavior.Cascade);
            modelbuilder.Entity<Applications>().HasOne(a => a.Applicant).WithMany(ap => ap.Applications).HasForeignKey(a => a.ApplicantId).OnDelete(DeleteBehavior.Restrict);
            modelbuilder.Entity<Job>().Property(j => j.CreatedAt).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Applications>().Property(a => a.AppliedAt).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Employer>().HasOne(e => e.User).WithOne().HasForeignKey<Employer>(e => e.UserId);
            modelbuilder.Entity<Applicant>().HasOne(e => e.User).WithOne().HasForeignKey<Applicant>(e => e.UserId);
            modelbuilder.Entity<Category>().HasMany(c=>c.Jobs).WithOne(j=>j.Category).HasForeignKey(j=>j.CategoryId);
            modelbuilder.Entity<Status>().HasMany(s=>s.Applications).WithOne(a=>a.Status).HasForeignKey(a=>a.StatusId);
            modelbuilder.Entity<WorkType>().HasMany(w=>w.Jobs).WithOne(j=>j.WorkType).HasForeignKey(j=>j.WorkTypeId);

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
