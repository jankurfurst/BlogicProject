using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;

namespace BlogicProject.Models.ViewModels
{
    public class ParticipantsViewModel
    {
        public IList<User> Clients { get; set; }
        public IList<User> Advisers { get; set; }
        public IList<User> Managers { get; set; }
        public IList<Institution> Institutions { get; set; }
    }
}
