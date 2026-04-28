using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class Part : WorkItem
    {
        [Required]
        [Display(Name = "Alkatrész mennyisége")]
        public int Quantity { get; set; }

        public int WorkProcessID { get; set; }
        public virtual WorkProcess WorkProcess { get; set; } = null!;
    }
}
