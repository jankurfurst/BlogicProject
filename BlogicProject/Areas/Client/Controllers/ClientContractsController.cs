using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles = nameof(Roles.Client))]
    public class ClientContractsController : Controller
    {
        ISecurityApplicationService iSecure;
        private readonly AppDbContext _context;

        public ClientContractsController(ISecurityApplicationService iSecure, AppDbContext context)
        {
            this.iSecure = iSecure;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            {
                if (User.Identity.IsAuthenticated)
                {
                    User currentUser = await iSecure.GetCurrentUser(User);
                    if (currentUser != null)
                    {
                        IList<Contract> clientContracts = await this._context.Contracts
                                                                            .Where(co => co.ClientID == currentUser.Id)
                                                                            .Include(c => c.Client)
                                                                            .Include(c => c.Manager)
                                                                            .Include(c => c.Institution)
                                                                            .Include(c => c.ParticipatesIn)
                                                                            .ThenInclude(pi => pi.User)
                                                                            .ToListAsync();
                        return View(clientContracts);
                    }
                }

                return NotFound();
            }
        }
    }
}
