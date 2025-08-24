using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Helpers;
using Rezk_Proj.Models;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("NearbyJobs")]
        public async Task<IActionResult> GetNearbyJobs(decimal applicantLatitude, decimal applicantLongitude)
        {
            if (applicantLatitude == 0 || applicantLongitude == 0)
                return BadRequest("Latitude and Longitude are required");

            var nearbyjobs = await _context.Jobs
                .Where(j => GeoHelper.CalculateDistance(applicantLatitude, applicantLongitude, j.Latitude, j.Longitude) <= 10)
                .ToListAsync();
          
            if(!nearbyjobs.Any())
                return NotFound("No nearby jobs found within 10 km radius.");

            return Ok(nearbyjobs);

        }
    }
}
