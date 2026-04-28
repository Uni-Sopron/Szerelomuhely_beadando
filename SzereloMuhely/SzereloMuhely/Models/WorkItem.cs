using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public abstract class WorkItem
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Név")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Ár")]
        public decimal Price { get; set; }
    }
}
