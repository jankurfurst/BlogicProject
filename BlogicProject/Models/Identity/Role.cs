using Microsoft.AspNetCore.Identity;

namespace BlogicProject.Models.Identity
{
    public class Role : IdentityRole<int>
    {
        public Role() : base() { }
        public Role(string role) : base(role) { }
    }
}
