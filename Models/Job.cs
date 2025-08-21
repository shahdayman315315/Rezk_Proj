using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public enum WorkType
    {
        FullTime = 1,
        PartTime,
        Remote
    }
    public class Job
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public Employer Employer { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, MaxLength(255)]
        public string LocationString { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }
        public decimal Salary { get; set; }

        [Required]
        public WorkType workType { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Applications> Applications { get; set; }

    }
}
