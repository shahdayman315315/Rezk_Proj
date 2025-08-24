using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ImageURL { get; set; }
        public List<Job> Jobs { get; set; }
    }
}
