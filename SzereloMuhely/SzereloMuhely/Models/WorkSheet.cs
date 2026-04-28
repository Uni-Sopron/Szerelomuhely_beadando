using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class WorkSheet
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Munkalap címe")]
        public string Title { get; set; } = null!;
        [Required]
        [Display(Name = "Szerelő ID")]
        public int MechanicID { get; set; }
        [Required]
        [Display(Name = "Munkalap státusza")]
        public bool Status { get; set; } = true;

        public bool IsClosed => !Status;

        [Required]
        [Display(Name = "Munkafelvevő")]
        public string RecruiterName { get; set; } = null!;

        [Required]
        [Display(Name = "Felvétel időpontja")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Fizetés módja")]
        public string? PaymentMethod { get; set; } = null!;

        [Display(Name = "Összesített ár")]
        public decimal TotalAmount
        {
            get
            {
                if (WorkProcesses == null) return 0;
                return WorkProcesses.Sum(wp => wp.Price + wp.Materials.Sum(m => m.Price * m.Quantity) + wp.Parts.Sum(p => p.Price * p.Quantity));
            }
        }

        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual ICollection<WorkProcess> WorkProcesses { get; set; } = null!;
    }
}
