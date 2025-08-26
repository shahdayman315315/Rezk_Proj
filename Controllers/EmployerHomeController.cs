using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Models;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerHomeController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetEmployerDashboard()
        {

            var employer = await context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var jobs=await context.Jobs
                .Where(j => j.EmployerId == employer.Id)
                .Include(j => j.Applications)
                .ThenInclude(a => a.Applicant)
                .ToListAsync();
            var result = jobs.Select(j =>new
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
                    a.Status,
                    a.AppliedAt

                }).ToList()
            });

            return Ok(result);
        }

        [HttpPost("AddJob")]    
        public async Task<IActionResult> AddJob([FromBody] Job job)
        {
           if(!ModelState.IsValid) 
                return BadRequest(ModelState);

           var employer=await context.Employers.FirstOrDefaultAsync(e=>e.UserId==User.FindFirst("uid").Value);

            job.EmployerId = employer.Id;
            await context.Jobs.AddAsync(job);
            await context.SaveChangesAsync();
            return Ok(job);
        }

        [HttpPut("UpdateJob/{id}")]
        public async Task<IActionResult> UpdateJob([FromRoute] int id, [FromBody] Job updatedJob)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(id!=updatedJob.Id)
                return BadRequest("Job ID mismatch");

            var employer = await context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var existingJob = await context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (existingJob is null)
                return NotFound(new { message = "There is no Job with this ID for this Employer" });

           
            existingJob.Title = updatedJob.Title;
            existingJob.Description = updatedJob.Description;
            existingJob.LocationString = updatedJob.LocationString;
            existingJob.Latitude = updatedJob.Latitude;
            existingJob.Longitude = updatedJob.Longitude;
            existingJob.MinSalary = updatedJob.MinSalary;
            existingJob.MaxSalary = updatedJob.MaxSalary;
            existingJob.WorkType = updatedJob.WorkType;
            existingJob.CategoryId = updatedJob.CategoryId;

            await context.SaveChangesAsync();

            return Ok(updatedJob);
        }

        [HttpDelete("DeleteJob/{id}")]
        public async Task<IActionResult> DeleteJob([FromRoute] int id)
        {
            var employer = await context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
          var job=await context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employer.Id);

            if (job is null)
                return NotFound(new { message = "There is no Job with this ID for this Employer" });

             context.Jobs.Remove(job);
            await context.SaveChangesAsync();
            return Ok(new { message = "Job deleted successfully" });
        }

        [HttpPut("UpdateApplicationStatus/{jobId}/{applicantId}")]
        public async Task<IActionResult> UpdateApplicationStatus([FromRoute] int jobId, [FromRoute] int applicantId, [FromBody] Status newStatus)
        {
            var employer = await context.Employers.FirstOrDefaultAsync(e => e.UserId == User.FindFirst("uid").Value);
            var job = await context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employer.Id);
            if (job is null)
                return NotFound(new { message = "There is no Job with this ID for this Employer" });

            var application = await context.Applications.FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicantId);
            if (application is null)
                return NotFound(new { message = "There is no Application with this Job ID and Applicant ID" });

            application.Status = newStatus;
            await context.SaveChangesAsync();
            return Ok(new { message = "Application status updated successfully" });
        }
    }
}
