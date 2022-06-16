using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using BlogicProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Areas.Adviser.Controllers
{
    [Area("Adviser")]
    [Authorize(Roles = nameof(Roles.Adviser))]
    public class ClientsController : Controller
    {

        private readonly UserManager<User> _userManager;
        ISecurityApplicationService iSecure;
        private readonly AppDbContext _context;

        public ClientsController(UserManager<User> usermanager, ISecurityApplicationService iSecure, AppDbContext context)
        {
            this.iSecure = iSecure;
            _userManager = usermanager;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                User currentUser = await iSecure.GetCurrentUser(User);

                if (currentUser == null)
                {
                    return NotFound();
                }
                var contracts = _context.Contracts.Where(c => c.ManagerID == currentUser.Id).ToList();

                List<User> users = new();
                foreach (var cont in contracts)
                {
                    var u = await _context.Users.Include(c => c.ParticipatesIn)
                                               .ThenInclude(pi => pi.Contract)
                                               .FirstOrDefaultAsync(u => u.Id == cont.ClientID);
                    users.Add(u);
                }
                List<UserSimpleDetailViewModel> vms = new();
                foreach (var user in users)
                {
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
                    vms.Add(viewModel);
                }
                return View(vms);

            }

            return NotFound();
        }
    }
}
