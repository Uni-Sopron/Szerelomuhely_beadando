using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class Material
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("Anyag neve")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Anyag mennyisége")]
        public int Quantity { get; set; }

        public int WorkProcessID { get; set; }
        public virtual WorkProcess WorkProcess{ get; set; }
    }
}
