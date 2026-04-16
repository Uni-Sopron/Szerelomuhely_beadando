using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("Felhasználónév")]
        public string Username { get; set; }
        [Required]
        [DisplayName("Jelszó")]
        public string Password { get; set; }
    }
}
