using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public class RegisterModel
    {
        [Required, MaxLength(100)]
        public string UserName { get; set; }

        [Required, MaxLength(14)]
        public string NationalId { get; set; }

        [Required, MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string LocationString { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

    }
}
