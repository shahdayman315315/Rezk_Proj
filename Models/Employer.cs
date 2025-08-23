using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Rezk_Proj.Models
{
    public class Employer
    {
        public int Id { get; set; }

        [Required,Column(TypeName = "varchar(14)")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National Id must be exactly 14 digits")]
        public string NationalId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, Column(TypeName = "varchar(11)")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(255)]
        public string LocationString { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }
        public List<Job> Jobs { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

    }
}
