using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogicProject.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username => Email.Substring(0, Email.IndexOf("@"));
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression(@"\d{2}((0|2)[1-9]|(1|3)[0-2]|(5|7)[1-9]|(6|8)[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])\/?\d{3,4}", ErrorMessage = "Rodné číslo je ve špatném formátu.")]
        public string PI_Number { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        //[UniqueCharacters(6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{4,}$", ErrorMessage = RegisterViewModel.ErrorMessagePassword)]
        public string Password { get; set; }
        private const string ErrorMessagePassword = "Passwords must be at least 4 characters." +
                                                    "\nPasswords must have at least one non alphanumeric character." +
                                                    "\nPasswords must have at least one digit('0'-'9')." +
                                                    "\nPasswords must have at least one uppercase('A'-'Z').";

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match!")]
        public string RepeatedPassword { get; set; }
    }
}
