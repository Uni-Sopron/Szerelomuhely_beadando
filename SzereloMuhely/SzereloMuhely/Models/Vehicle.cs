using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class Vehicle
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Rendszám")]
        public string LicensePlate { get; set; } = null!;
        [Required]
        [Display(Name = "Gyártó")]
        public string Make { get; set; } = null!;
        [Required]
        [Display(Name = "Típus")]
        public string Model { get; set; } = null!;
        [Required]
        [Display(Name = "Tulajdonos neve")]
        public string OwnerName { get; set; } = null!;
        [Required]
        [Display(Name = "Tulajdonos címe")]
        public string OwnerAddress { get; set; } = null!;


        public int WorkSheetID { get; set; }
        public virtual WorkSheet WorkSheet { get; set; } = null!;
    }
}
