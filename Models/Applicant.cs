using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public class Applicant
    {
        public int Id { get; set; }

        [Required, MaxLength(14)]
        public string NationalId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(255)]
        public string LocationString { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }
        public List<Applications> Applications { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

    }
}
