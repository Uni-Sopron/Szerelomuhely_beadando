using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class Part
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("Alkatrész neve")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Alkatrész mennyisége")]
        public int Quantity { get; set; }

        public int WorkProcessID { get; set; }
        public virtual WorkProcess WorkProcess { get; set; }
    }
}
