using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Helpers;
using Rezk_Proj.Models;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantHomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ApplicantHomeController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
