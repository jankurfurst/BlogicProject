using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin))]
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ContractList()
        {
            var contracts = await _context.Contracts.Include(c => c.Manager)
                                                    .Include(c => c.Client)
                                                    .Include(c => c.Institution)
                                                    .Include(c => c.ParticipatesIn)
                                                    .ThenInclude(pi => pi.User)
                                                    .ToListAsync();
            return View(contracts);
        }
    }
}
