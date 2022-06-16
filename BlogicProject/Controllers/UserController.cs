using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using BlogicProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Adviser) + ", " + nameof(Roles.Client))]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        ISecurityApplicationService iSecure;
        private readonly AppDbContext _context;

        public UserController(UserManager<User> usermanager, ISecurityApplicationService iSecure, AppDbContext context)
        {
            _userManager = usermanager;
            this.iSecure = iSecure;
            _context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(c => c.ParticipatesIn)
                                           .ThenInclude(pi => pi.Contract)
                                           .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var contractsClient = _context.Contracts.Where(c => c.Client == user).ToList();
            var contractsManager = _context.Contracts.Where(c => c.Manager == user).ToList();
            var participates = _context.Participatings.Where(p => p.UserID == user.Id).ToList();
            var roles = _userManager.GetRolesAsync(user).Result.ToList();
            

            UserSimpleDetailViewModel viewModel = new()
            {
                User = user,
                UserRoles = roles,
                ContractsClient = contractsClient,
                ContractsManager = contractsManager,
                Participatings = user.ParticipatesIn
            };

            return View(viewModel);
        }
    }
}
