using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SzereloMuhely.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Felhasználónév")]
        public string Username { get; set; } = null!;
        [Required]
        [Display(Name = "Jelszó")]
        public string Password { get; set; } = null!;
    }
}
