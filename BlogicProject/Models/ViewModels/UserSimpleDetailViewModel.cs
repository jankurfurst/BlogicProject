using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;

namespace BlogicProject.Models.ViewModels
{
    public class UserSimpleDetailViewModel
    {
        public User User { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<Contract> ContractsClient { get; set; }
        public IList<Contract> ContractsManager { get; set; }
        public IList<Participating> Participatings { get; set; }
    }
}
