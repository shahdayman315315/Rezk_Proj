using System.ComponentModel.DataAnnotations;

namespace Rezk_Proj.Models
{
    public class Status
    {
        public int Id { get; set; }

        [Required]
        public string NameEn { get; set; }

        [Required]
        public string NameAr { get; set; }
        public List<Applications> Applications { get; set; } = new();

    }
}
