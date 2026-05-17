using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SzereloMuhely.Models
{
    public class WorkSheet
    {
        [Display(Name = "Azonosító")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Munkalap címe")]
        public string Title { get; set; } = null!;
        [Required]
        [Display(Name = "Szerelő")]
        public string MechanicID { get; set; } = null!;
        [Required]
        [Display(Name = "Munkalap nyitott")]
        public bool IsOpen { get; set; } = true;


        [Required]
        [Display(Name = "Munkafelvevő")]
        public string RecruiterId { get; set; } = null!;

        [Required]
        [Display(Name = "Felvétel időpontja")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Fizetés módja")]
        public string? PaymentMethod { get; set; } = null;

        [Display(Name = "Összesített ár")]
        public int TotalAmount
        {
            get
            {
                if (WorkProcesses == null) return 0;
                return WorkProcesses.Sum(wp => 
                    wp.Price + 
                    (wp.Materials?.Sum(m => m.Price * m.Quantity) ?? 0) + 
                    wp.Parts?.Sum(p => p.Price * p.Quantity) ?? 0);
            }
        }
        [ValidateNever]
        [NotMapped]
        [Display(Name = "Szerelő")]
        public virtual IdentityUser? Mechanic { get; set; }
        [ValidateNever]
        [NotMapped]
        [Display(Name = "Munkafelvevő")]
        public virtual IdentityUser? Recruiter { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
        public virtual ICollection<WorkProcess> WorkProcesses { get; set; } = new List<WorkProcess>();
    }
}
