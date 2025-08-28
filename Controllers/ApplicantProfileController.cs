using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ApplicantProfileController> _logger;

        public ApplicantProfileController(
            ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<ApplicantProfileController> logger)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/ApplicantProfile
        [HttpGet("GetAllApplicants")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllApplicants()
        {
            var applicants = await _context.Applicants
                .AsNoTracking()
                .Select(a => new
                {
                    a.Id,
                    a.NationalId,
                    a.Name,
                    a.PhoneNumber,
                    a.LocationString,
                    OwnerUserId = a.UserId 
                })
                .ToListAsync();

            return Ok(applicants);
        }

        // GET: api/ApplicantProfile/5
        [HttpGet("GetApplicantById/{id}")]
        public async Task<IActionResult> GetApplicantById([FromRoute] int id)
        {
            _logger.LogInformation("GetApplicantById: request received for id={Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("GetApplicantById: invalid id={Id}", id);
                return BadRequest(new { message = "Invalid applicant id" });
            }

            var applicant = await _context.Applicants
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
            {
                _logger.LogWarning("GetApplicantById: applicant not found id={Id}", id);
                return NotFound(new { message = "Applicant not found" });
            }

            _logger.LogInformation("GetApplicantById: applicant found id={Id}, userId={UserId}",
                applicant.Id, applicant.UserId);

            return Ok(new
            {
                applicant.Id,
                applicant.NationalId,
                applicant.Name,
                applicant.PhoneNumber,
                applicant.LocationString,
                applicant.Latitude,
                applicant.Longitude,
                OwnerUserId = applicant.UserId
            });
        }

        public record ApplicantUpdateDto(string NationalId, string Name, string PhoneNumber, string LocationString, decimal Latitude, decimal Longitude);

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateApplicant(int id, [FromBody] ApplicantUpdateDto dto)
        {
            var existingApplicant = await _context.Applicants.FindAsync(id);
            if (existingApplicant == null)
                return NotFound(new { message = "Applicant not found" });

            // update fields
            existingApplicant.NationalId = dto.NationalId;
            existingApplicant.Name = dto.Name;
            existingApplicant.PhoneNumber = dto.PhoneNumber;
            existingApplicant.LocationString = dto.LocationString;
            existingApplicant.Latitude = dto.Latitude;
            existingApplicant.Longitude = dto.Longitude;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Applicant updated successfully" });
        }


        // DELETE: api/ApplicantProfile/DeleteApplicant/{id}
        [HttpDelete("DeleteApplicant/{id}")]
        public async Task<IActionResult> DeleteApplicant([FromRoute] int id)
        {
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
            {
                return NotFound(new { message = "Applicant not found" });
            }

            try
            {
                _context.Applicants.Remove(applicant);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Applicant and related applications deleted successfully" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Delete failed due to database constraint",
                    details = ex.InnerException?.Message
                });
            }
        }



        // POST: api/ApplicantProfile/signout
        [HttpPost("SignOutApplicant")]
        public async Task<IActionResult> SignOutApplicant()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("SignOutUser: user signed out");
            return Ok(new { message = "Signed out successfully." });
        }
    }

   
}
