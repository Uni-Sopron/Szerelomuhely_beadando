using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class WorkSheet
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("Munkalap címe")]
        public string Title { get; set; }
        [Required]
        [DisplayName("Szerelő ID")]
        public int MechanicID { get; set; }
        [Required]
        [DisplayName("Munkalap státusza")]
        public bool Status { get; set; }
        [DisplayName("Fizetés módja")]
        public string PaymentMethod { get; set; }

        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<WorkProcess> WorkProcesses { get; set; }
    }
}
