using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rezk_Proj.Models;

namespace Rezk_Proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryJobs(int id)
        {
            var category = _context.Categories.Find(id) ;
            if (category is null)
            {
                return NotFound("There is no Category with this ID");
            }
            var jobsInCategory = _context.Jobs.Where(j => j.CategoryId == id).ToList(); 

            return Ok(jobsInCategory);
        }
    }
}
