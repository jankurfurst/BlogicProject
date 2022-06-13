using BlogicProject.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogicProject.Models.Entity
{
    [Table(nameof(Participating))]
    public class Participating
    {
        [Key]
        [Required]
        public int ID { get; set; }

        public int ContractID { get; set; }
        public Contract Contract { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }
}
