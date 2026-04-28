using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SzereloMuhely.Models
{
    public class WorkProcess : WorkItem
    {
        [Required]
        [Display(Name = "Munkafolyamat időtartama")]
        public int Duration { get; set; }

        public int WorkSheetID { get; set; }
        public virtual WorkSheet WorkSheet { get; set; } = null!;

        public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
