using BlogicProject.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogicProject.Models.Entity
{
    [Table(nameof(Contract))]
    public class Contract
    {
        [Key]
        [Required]
        public int RegistrationNumber { get; set; }

        [Required]
        public DateTime ConclusionDate { get; set; }

        [Required]
        public DateTime EfectiveDate { get; set; }

        [Required]
        public DateTime ExpiredDate { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [ForeignKey(nameof(User))]
        public int ManagerID { get; set; }
        public User Manager { get; set; }

        [ForeignKey(nameof(User))]
        public int ClientID { get; set; }
        public User Client { get; set; }

        public IList<Participating> ParticipatesIn { get; set; }
    }
}
