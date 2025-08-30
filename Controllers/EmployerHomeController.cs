using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Models;
using Rezk_Proj.Services;
using System.Security.Claims;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployerHomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public EmployerHomeController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetEmployerDashboard()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(u => u.UserId == userId);
            //var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var jobs=await _context.Jobs
                .Where(j => j.EmployerId == employer.Id)
                .Include(j => j.Applications)
                .ThenInclude(a => a.Applicant)
                .ToListAsync();
            var result = jobs.Select(j => new
            {
                j.Id,
                j.Title,
                j.Description,
                j.LocationString,
                j.MinSalary,
                j.MaxSalary,
                j.WorkType,
                j.CreatedAt,
                ApplicantsCount= j.Applications.Count,
                Applications = j.Applications.Select(a => new
                {
                    a.Applicant.Id,
                    a.Applicant.NationalId,
                    a.Applicant.Name,
                    a.Applicant.PhoneNumber,
                    a.Applicant.LocationString,
                    a.Applicant.Latitude,
                    a.Applicant.Longitude,
                    a.StatusId,
                    a.AppliedAt

                }).ToList()
            });

            return Ok(result);
        }

        public record JobDto(string Title, string Description, string LocationString, decimal Latitude, decimal Longitude, float MinSalary, float MaxSalary, int WorkTypeId, int CategoryId);
        [HttpPost("AddJob")]    
        public async Task<IActionResult> AddJob([FromBody] JobDto job)
        {
           if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(u => u.UserId == userId);
            // var employer=await _context.Employers.FirstOrDefaultAsync(e=>e.UserId==User.FindFirst("uid").Value);

            var newJob = new Job
            {
                Title = job.Title,
                Description = job.Description,
                LocationString = job.LocationString,
                Latitude = job.Latitude,
                Longitude = job.Longitude,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                WorkTypeId = job.WorkTypeId,
                CategoryId = job.CategoryId,
                EmployerId = employer.Id
            };
            await _context.Jobs.AddAsync(newJob);
            await _context.SaveChangesAsync();
            return Ok(job);
        }

        [HttpPut("UpdateJob/{id}")]
        public async Task<IActionResult> UpdateJob([FromRoute] int id, [FromBody] JobDto updatedJob)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            //var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(u => u.UserId == userId);
            var existingJob = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (existingJob is null)
                return NotFound(new { message = "There is no Job with this ID for this Employer" });

           
            existingJob.Title = updatedJob.Title;
            existingJob.Description = updatedJob.Description;
            existingJob.LocationString = updatedJob.LocationString;
            existingJob.Latitude = updatedJob.Latitude;
            existingJob.Longitude = updatedJob.Longitude;
            existingJob.MinSalary = updatedJob.MinSalary;
            existingJob.MaxSalary = updatedJob.MaxSalary;
            existingJob.WorkTypeId = updatedJob.WorkTypeId;
            existingJob.CategoryId = updatedJob.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(updatedJob);
        }

        [HttpDelete("DeleteJob/{id}")]
        public async Task<IActionResult> DeleteJob([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(u => u.UserId == userId);
            //var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var job=await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (job is null)
                return NotFound(new { message = "There is no Job with this ID for this Employer" });

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Job deleted successfully" });
        }

        [HttpPut("UpdateApplicationStatus/{jobId}/{applicantId}/{newStatus}")]
        public async Task<IActionResult> UpdateApplicationStatus([FromRoute] int jobId, [FromRoute] int applicantId, [FromRoute] int newStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employer = await _context.Employers.FirstOrDefaultAsync(u => u.UserId == userId);
            //var employer = await _context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employer.Id);
            if (job is null)
                return NotFound(new { message = "There is no Job with this ID for this Employer" });

            var application = await _context.Applications.Include(a=>a.Applicant).ThenInclude(ap=>ap.User).FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicantId);
            if (application is null)
                return NotFound(new { message = "There is no Application with this Job ID and Applicant ID" });

            application.StatusId = newStatus;
            await _context.SaveChangesAsync();

            var status = await _context.StatusTypes
                    .Where(s => s.Id == newStatus)
                    .Select(s => s.NameAr)
                    .FirstOrDefaultAsync();

            string applicantEmail = application.Applicant.User.Email;
            
            string subject = $"تم تحديث حالة طلبك لوظيفة {job.Title}";
            string body = $@"
<div style='font-family: Arial, sans-serif; line-height:1.8; color:#333;'>
  <h2 style='color:#2c3e50; margin-bottom:20px;'>مرحباً {application.Applicant.Name},</h2>
  
  <p style='font-size:16px; margin-bottom:20px;'>
    تم تحديث حالة طلبك للوظيفة 
    <b style='color:#2980b9;'>{job.Title}</b> 
    إلى: 
    <b style='color:#27ae60;'>{status}</b>.
  </p>
  
  <br/>
  
  <p style='font-size:14px; color:#555; margin-top:30px;'>
    مع أطيب التحيات،<br/>
    <span style='color:#27ae60; font-weight:bold;'>فريق بوابة التوظيف</span>
  </p>
</div>
";


            await _emailService.SendEmailAsync(applicantEmail, subject, body);

            return Ok(new { message = "Application status updated successfully and notification email sent" });
        }
    }
    
}
