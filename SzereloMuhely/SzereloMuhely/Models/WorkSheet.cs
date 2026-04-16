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
        public bool Status { get; set; }
        [Display(Name = "Fizetés módja")]
        public string PaymentMethod { get; set; } = null!;

        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual ICollection<WorkProcess> WorkProcesses { get; set; } = null!;
    }
}
