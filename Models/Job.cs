using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public enum WorkType
    {
        FullTime = 1,
        PartTime = 2,
        Remote = 3
    }
    public enum Categories
    {
        Supermarket = 1,
        Education = 2,
        Phrmacy = 3,
        Restuarant = 4,
        Management = 5,
        ManualWork = 6,
        ExternalWork = 7,
        Security = 8,
        Crafts = 9,
        Transportation = 10,
        HomeServices = 11,
        Farming = 12
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

        [Required]
        public Categories Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Applications> Applications { get; set; }

    }
}
