using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class Vehicle
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("Rendszám")]
        public string LicensePlate { get; set; }
        [Required]
        [DisplayName("Gyártó")]
        public string Make { get; set; }
        [Required]
        [DisplayName("Típus")]
        public string Model { get; set; }
        [Required]
        [DisplayName("Tulajdonos neve")]
        public string OwnerName { get; set; }
        [Required]
        [DisplayName("Tulajdonos címe")]
        public string OwnerAddress { get; set; }


        public int WorkSheetID { get; set; }
        public virtual WorkSheet WorkSheet { get; set; }
    }
}
