using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
