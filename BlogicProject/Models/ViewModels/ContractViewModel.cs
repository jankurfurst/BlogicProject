using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogicProject.Models.ViewModels
{
    public class ContractViewModel
    {
        [Required]
        public int RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Contract must have Date of Conclusion!")]
        public DateTime ConclusionDate { get; set; }

        [Required(ErrorMessage = "Contract must have Efective Date!")]
        public DateTime EfectiveDate { get; set; }

        [Required(ErrorMessage = "Contract must have Date of Expire!")]
        public DateTime ExpiredDate { get; set; }

        [Required(ErrorMessage = "Contract must have Institution!")]
        public int InstitutionId { get; set; }

        [Required(ErrorMessage = "Contract must have Manager!")]
        public int ManagerId { get; set; }

        [Required(ErrorMessage = "Contract must have Client!")]
        public int ClientId { get; set; }


        public IList<int> ParticipatingAdvisers { get; set; }

    }
}
