using BlogicProject.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlogicProject.Models.Identity
{
    [Index(nameof(PI_Number),IsUnique = true)]
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [StringLength(10)]
        public string PI_Number { get; set; } // personal identification number (rodné číslo)

        public int Age { get; set; }

        public IList<Participating> ParticipatesIn { get; set; }
    }
}
