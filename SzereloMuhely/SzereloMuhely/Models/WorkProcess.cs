using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class WorkProcess : WorkItem
    {
        [Required]
        [Display(Name = "Munkaóra")]
        public int Duration { get; set; }

        public int WorkSheetID { get; set; }
        [Display(Name = "Munkalap")]
        public virtual WorkSheet? WorkSheet { get; set; }

        public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
