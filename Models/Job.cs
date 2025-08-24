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

        [Required]
        public int EmployerId { get; set; }
        public Employer Employer { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(1000)]
        public string Description { get; set; }

        [Required, MaxLength(255)]
        public string LocationString { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Range(0, double.MaxValue)]
        public double Salary { get; set; }

        [Required]
        public WorkType WorkType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Applications> Applications { get; set; } = new();
    }

}
