using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public class WorkType
    {
        public int Id { get; set; }

        [Required]
        public string ArabicLabel { get; set; }

        [Required]
        public string EnglishLabel { get; set; }
        public List<Job> Jobs { get; set; } = new();
    }
}
