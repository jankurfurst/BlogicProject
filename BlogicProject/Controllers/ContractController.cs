using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Adviser) + ", " + nameof(Roles.Client))]
    public class ContractController : Controller
    {
        private readonly AppDbContext _context;

        public ContractController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.Include(c => c.Manager)
                                            .Include(c => c.Client)
                                            .Include(c=>c.Institution)
                                            .Include(c => c.ParticipatesIn)
                                            .ThenInclude(pi => pi.User)
                                            .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
    }
}
