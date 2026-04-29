using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class Material : WorkItem
    {
        [Required]
        [Display(Name = "Anyag mennyisége")]
        public int Quantity { get; set; }

        public int WorkProcessID { get; set; }
        public virtual WorkProcess WorkProcess { get; set; } = null!;
    }
}
