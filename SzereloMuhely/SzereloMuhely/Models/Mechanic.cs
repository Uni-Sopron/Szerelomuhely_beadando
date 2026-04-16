namespace SzereloMuhely.Models
{
    public class Mechanic:User
    {
        public virtual ICollection<WorkSheet> WorkSheets { get; set; }
    }
}
