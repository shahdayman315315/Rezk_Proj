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
    [Authorize]
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

        //// GET: api/ApplicantProfile
        //[HttpGet("GetAllApplicants")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetAllApplicants()
        //{
        //    var applicants = await _context.Applicants
        //        .AsNoTracking()
        //        .Select(a => new
        //        {
        //            a.Id,
        //            a.NationalId,
        //            a.Name,
        //            a.PhoneNumber,
        //            a.LocationString,
        //            OwnerUserId = a.UserId 
        //        })
        //        .ToListAsync();

        //    return Ok(applicants);
        //}

        // GET: api/ApplicantProfile/5
        [HttpGet("GetApplicantProfile")]
        public async Task<IActionResult> GetApplicantById()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicant = await _context.Applicants.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);

            if (applicant == null)
            {
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

        [HttpPut("UpdateApplicantProfile")]
        public async Task<IActionResult> UpdateApplicant( [FromBody] ApplicantUpdateDto dto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicant = await _context.Applicants.FirstOrDefaultAsync(u => u.UserId == userId);

            // update fields
            applicant.NationalId = dto.NationalId;
            applicant.Name = dto.Name;
            applicant.PhoneNumber = dto.PhoneNumber;
            applicant.LocationString = dto.LocationString;
            applicant.Latitude = dto.Latitude;
            applicant.Longitude = dto.Longitude;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Applicant updated successfully" });
        }


        // DELETE: api/ApplicantProfile/DeleteApplicant/{id}
        [HttpDelete("DeleteApplicant")]
        public async Task<IActionResult> DeleteApplicant()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicant = await _context.Applicants.Include(a => a.Applications).FirstOrDefaultAsync(u => u.UserId == userId);

            

            if (applicant == null)
                return NotFound();

            if (applicant.Applications.Any())
                _context.Applications.RemoveRange(applicant.Applications);

            _context.Applicants.Remove(applicant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Applicant and related applications deleted successfully" });


            
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
