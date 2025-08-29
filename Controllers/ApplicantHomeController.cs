using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Helpers;
using Rezk_Proj.Models;
using Rezk_Proj.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicantHomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public ApplicantHomeController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        
        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToArrayAsync();
            var result = categories.Select(c => new
            {
                c.Id,
                c.Name,
                ImageUrl = $"{Request.Scheme}://{Request.Host}/images/categories/{c.ImageURL}"

            });
            return Ok(result);
        }


        [HttpGet("CategoryJobs/{id}")]
        public async Task<IActionResult> GetCategoryJobs(int id)
        {
            var category = await _context.Categories
                           .Include(c => c.Jobs)
                           .FirstOrDefaultAsync(c => c.Id== id);

            if (category is null)
                return NotFound(new { message = "There is no Category with this ID" });
            var jobs=category.Jobs.Select(j => new
            {
                j.Id,
                j.Title,
                j.Description,
                j.LocationString,
                j.MinSalary,
                j.MaxSalary,
                j.WorkTypeId,
                j.CreatedAt,
               
            });

            return Ok(jobs);
        }

        [HttpGet("NearbyJobs")]
        public async Task<IActionResult> GetNearbyJobs( )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicant = await _context.Applicants.FirstOrDefaultAsync(u => u.UserId == userId);

            //var nearbyjobs = await _context.Jobs
            //    .Where(j => GeoHelper.CalculateDistance(applicantLatitude, applicantLongitude, j.Latitude, j.Longitude) <= 10)
            //    .Select(j => new
            //    {
            //        j.Id,
            //        j.Title,
            //        j.Description,
            //        j.LocationString,
            //        j.MinSalary,
            //        j.MaxSalary,
            //        j.WorkType,
            //        j.CreatedAt,
            //    })
            //    .ToListAsync();
            var jobs = await _context.Jobs
    .Select(j => new
    {
        j.Id,
        j.Title,
        j.Description,
        j.LocationString,
        j.MinSalary,
        j.MaxSalary,
        j.WorkType,
        j.CreatedAt,
        j.Latitude,
        j.Longitude
    })
    .ToListAsync();

            var nearbyjobs = jobs
                .Where(j => GeoHelper.CalculateDistance(applicant.Latitude, applicant.Longitude, j.Latitude, j.Longitude) <= 10)
                .ToList();
            Console.WriteLine(GeoHelper.CalculateDistance(30.69m, 30.31m, 30.70m, 30.36m));
            Console.WriteLine(GeoHelper.CalculateDistance(30.69m, 30.31m, 30.65m, 30.28m));
            Console.WriteLine(GeoHelper.CalculateDistance(30.69m, 30.31m, 30.73m, 30.34m));
            Console.WriteLine(GeoHelper.CalculateDistance(30.69m, 30.31m, 30.68m, 30.29m));
            Console.WriteLine(GeoHelper.CalculateDistance(30.69m, 30.31m, 30.72m, 30.30m));



            if (!nearbyjobs.Any())
                return NotFound("No nearby jobs found within 10 km radius.");

            return Ok(nearbyjobs);

        }


        
        [HttpPost("ApplyToJob/{jobId}")]
        public async Task<IActionResult> ApplyToJob([FromRoute] int jobId)
        {
            var userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
          var applicant = await _context.Applicants.FirstOrDefaultAsync(u => u.UserId == userId);
            // var applicant = await _context.Applicants.FirstOrDefaultAsync(a => a.UserId == userId);
         //   var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        //    var employer = await _context.Employers.FirstOrDefaultAsync(e => e.Id== job.EmployerId);

            var job= await _context.Jobs.Include(j=>j.Employer).ThenInclude(e=>e.User).FirstOrDefaultAsync(j => j.Id == jobId);
            if (job is null)
                return NotFound(new { message = "There is no Job with this ID" });

            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicant.Id);

            if (existingApplication is not null)
                return BadRequest(new { message = "You have already applied to this job" });
           var status = await _context.StatusTypes.FirstOrDefaultAsync(s => s.Id==(int) Models.Enums.Status.Pending);

            await _context.Applications.AddAsync(new Applications
            {
                Job = job,
                Applicant = applicant,
                Status = status
            });

            await _context.SaveChangesAsync();
            string employerEmail = job.Employer.User.Email;
            //    string subject = $"New Application for {job.Title}";
            //    string body = $@"
            //<h2>Hello {job.Employer.Name},</h2>
            //<p>A new applicant has applied for your job <b>{job.Title}</b>.</p>
            //<p><b>Applicant Name:</b> {applicant.Name}</p>
            //<p><b>Applicant Email:</b> {User.FindFirst(JwtRegisteredClaimNames.Email)?.Value}</p>
            //<p>Status: Pending</p>
            //<br/>
            //<p>Regards,<br/>Your Job Portal</p>
            //   ";
            //            string subject = $"طلب تقديم جديد لوظيفة {job.Title}";
            //            string body = $@"
            //<h2>مرحباً {job.Employer.Name},</h2>
            //<p>لقد قام متقدم جديد بالتقديم على وظيفتك <b>{job.Title}</b>.</p>
            //<p><b>اسم المتقدم:</b> {applicant.Name}</p>
            //<p><b>البريد الإلكتروني للمتقدم:</b> {User.FindFirst(JwtRegisteredClaimNames.Email)?.Value}</p>
            //<p><b>الحالة:</b> قيد الانتظار</p>
            //<br/>
            //<p>مع أطيب التحيات،<br/>فريق بوابة التوظيف</p>
            string subject = $"طلب تقديم جديد لوظيفة {job.Title}";
            string body = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.8; color: #333;'>
  <h2 style='color: #2c3e50;'>مرحباً {job.Employer.Name},</h2>
  
  <p style='font-size: 16px;'>لقد قام متقدم جديد بالتقديم على وظيفتك:</p>
  <p style='background-color:#f8f9fa; padding:10px; border-left:4px solid #3498db;'>
    <b>الوظيفة:</b> {job.Title}
  </p>
  
  <p style='margin-top:20px; font-size:16px;'><b style='color:#16a085;'>اسم المتقدم:</b> {applicant.Name}</p>
  <p style='font-size:16px;'><b style='color:#2980b9;'>البريد الإلكتروني للمتقدم:</b> {User.FindFirst(JwtRegisteredClaimNames.Email)?.Value}</p>
  <p style='font-size:16px;'><b style='color:#e67e22;'>الحالة:</b> <span style='color:#d35400;'>قيد الانتظار</span></p>
  
  <hr style='margin:30px 0; border:none; border-top:1px solid #ccc;'/>
  
  <p style='font-size:14px; color:#555;'>مع أطيب التحيات،<br/>
  <span style='color:#27ae60; font-weight:bold;'>فريق بوابة التوظيف</span></p>
</div>
";
            //";

            await _emailService.SendEmailAsync(employerEmail, subject, body);

            return Ok(new { message = "Application submitted successfully and notification email sent." });
        
        
        }

        [HttpDelete("RemoveApplication/{jobId}")]
        public async Task<IActionResult> RemoveApplication([FromRoute] int jobId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicant = await _context.Applicants.FirstOrDefaultAsync(u => u.UserId == userId);

            if (applicant is null)
                return NotFound(new { message = "Applicant not found" });

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicant.Id);

            if (application is null)
                return NotFound(new { message = "No application found for this job" });

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application removed successfully" });
        }

    }
}
