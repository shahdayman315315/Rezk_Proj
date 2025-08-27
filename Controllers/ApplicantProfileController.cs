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
                    OwnerUserId = a.UserId // helpful for debugging ownership issues
                })
                .ToListAsync();

            return Ok(applicants);
        }

        // GET: api/ApplicantProfile/5
        [HttpGet("GetApplicantById")]
        public async Task<IActionResult> GetApplicantById([FromRoute] int id)
        {
            var applicant = await _context.Applicants
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
                return NotFound(new { message = "Applicant not found" });

            return Ok(new
            {
                applicant.Id,
                applicant.NationalId,
                applicant.Name,
                applicant.PhoneNumber,
                applicant.LocationString,
                applicant.Latitude,
                applicant.Longitude,
                OwnerUserId = applicant.UserId // return owner id for easier debugging on client side
            });
        }

        // PUT: api/ApplicantProfile/5
        [HttpPut("UpdateApplicant")]
        public async Task<IActionResult> UpdateApplicant([FromRoute] int id, [FromBody] Applicant dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateApplicant: invalid model state for id={Id}", id);
                return BadRequest(ModelState);
            }

            var existingApplicant = await _context.Applicants.FindAsync(id);
            if (existingApplicant == null)
            {
                _logger.LogInformation("UpdateApplicant: applicant not found id={Id}", id);
                return NotFound(new { message = "Applicant not found" });
            }

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("UpdateApplicant: unauthenticated attempt for id={Id}", id);
                return Unauthorized(new { message = "You must be authenticated to update an applicant." });
            }

            // try to get user id from claims or UserManager fallback
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) && _userManager != null)
            {
                currentUserId = _userManager.GetUserId(User);
            }

            var isAdmin = User.IsInRole("Admin") || User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");

            if (existingApplicant.UserId != currentUserId && !isAdmin)
            {
                _logger.LogWarning("UpdateApplicant: forbidden user={UserId} tries to update applicant={ApplicantUserId}", currentUserId, existingApplicant.UserId);
                return Forbid();
            }

            // apply updates
            existingApplicant.NationalId = dto.NationalId;
            existingApplicant.Name = dto.Name;
            existingApplicant.PhoneNumber = dto.PhoneNumber;
            existingApplicant.LocationString = dto.LocationString;
            existingApplicant.Latitude = dto.Latitude;
            existingApplicant.Longitude = dto.Longitude;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("UpdateApplicant: applicant {Id} updated by user {UserId}", id, currentUserId);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "UpdateApplicant: DB update error for applicant {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the applicant", error = ex.Message });
            }
        }

        // DELETE: api/ApplicantProfile/5
        [HttpDelete("DeleteApplicant")]
        public async Task<IActionResult> DealeteApplicant([FromRoute] int id)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null)
            {
                _logger.LogInformation("DeleteApplicant: not found id={Id}", id);
                return NotFound(new { message = "Applicant not found" });
            }

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("DeleteApplicant: unauthenticated attempt id={Id}", id);
                return Unauthorized(new { message = "You must be authenticated to delete an applicant." });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) && _userManager != null)
            {
                currentUserId = _userManager.GetUserId(User);
            }

            var isAdmin = User.IsInRole("Admin") || User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            if (applicant.UserId != currentUserId && !isAdmin)
            {
                _logger.LogWarning("DeleteApplicant: forbidden user={UserId} tries to delete owner={OwnerId}", currentUserId, applicant.UserId);
                return Forbid();
            }

            _context.Applicants.Remove(applicant);
            await _context.SaveChangesAsync();
            _logger.LogInformation("DeleteApplicant: applicant {Id} deleted by user {UserId}", id, currentUserId);
            return Ok(new { message = "Applicant deleted successfully" });
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
