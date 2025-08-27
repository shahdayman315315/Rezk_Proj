using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Helpers;
using Rezk_Proj.Models;
using Rezk_Proj.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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


        [HttpGet("CategoryJobs")]
        public async Task<IActionResult> GetCategoryJobs([FromRoute] int id)
        {
            var category = await _context.Categories
                           .Include(c => c.Jobs)
                           .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                return NotFound(new { message = "There is no Category with this ID" });

            return Ok(category.Jobs);
        }

        [HttpGet("NearbyJobs")]
        public async Task<IActionResult> GetNearbyJobs(decimal applicantLatitude, decimal applicantLongitude)
        {
            if (applicantLatitude == 0 || applicantLongitude == 0)
                return BadRequest("Latitude and Longitude are required");

            var nearbyjobs = await _context.Jobs
                .Where(j => GeoHelper.CalculateDistance(applicantLatitude, applicantLongitude, j.Latitude, j.Longitude) <= 10)
                .ToListAsync();

            if (!nearbyjobs.Any())
                return NotFound("No nearby jobs found within 10 km radius.");

            return Ok(nearbyjobs);

        }

        [HttpPost("ApplyToJob/{jobId}")]
        public async Task<IActionResult> ApplyToJob([FromRoute] int jobId)
        {
            var applicant = await _context.Applicants.FirstOrDefaultAsync(a => a.UserId == User.FindFirst("uid").Value);
            var job= await _context.Jobs.Include(j=>j.Employer).ThenInclude(e=>e.User).FirstOrDefaultAsync(j => j.Id == jobId);
            if (job is null)
                return NotFound(new { message = "There is no Job with this ID" });

            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicant.Id);

            if (existingApplication is not null)
                return BadRequest(new { message = "You have already applied to this job" });

            await _context.Applications.AddAsync(new Applications
            {
                Job = job,
                Applicant = applicant,
                Status = Status.Pending
            });

            await _context.SaveChangesAsync();
            string employerEmail = job.Employer.User.Email; 
            string subject = $"New Application for {job.Title}";
            string body = $@"
        <h2>Hello {job.Employer.Name},</h2>
        <p>A new applicant has applied for your job <b>{job.Title}</b>.</p>
        <p><b>Applicant Name:</b> {applicant.Name}</p>
        <p><b>Applicant Email:</b> {User.FindFirst(JwtRegisteredClaimNames.Email)?.Value}</p>
        <p>Status: Pending</p>
        <br/>
        <p>Regards,<br/>Your Job Portal</p>
           ";

            await _emailService.SendEmailAsync(employerEmail, subject, body);

            return Ok(new { message = "Application submitted successfully and notification email sent." });
        
        }
    }
}
