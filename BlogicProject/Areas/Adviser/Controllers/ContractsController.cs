using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogicProject.Areas.Adviser.Controllers
{
    [Area("Adviser")]
    [Authorize(Roles = nameof(Roles.Adviser))]
    public class ContractsController : Controller
    {
        ISecurityApplicationService iSecure;
        private readonly AppDbContext _context;

        public ContractsController(ISecurityApplicationService iSecure, AppDbContext context)
        {
            this.iSecure = iSecure;
            _context = context;
        }

        public async Task<IActionResult> ManagingContracts()
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            if (currentUser != null)
            {
                IList<Contract> managerContracts = await this._context.Contracts
                                                                    .Where(co => co.ManagerID == currentUser.Id)
                                                                    .Include(c => c.Manager)
                                                                    .Include(c => c.Client)
                                                                    .Include(c => c.Institution)
                                                                    .Include(c => c.ParticipatesIn)
                                                                    .ThenInclude(pi => pi.User)
                                                                    .ToListAsync();
                return View(managerContracts);
            }
            return NotFound();
        }
        
        public async Task<IActionResult> ParticipatingContracts()
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            if (currentUser != null)
            {
                var participatings = await this._context.Participatings
                                                                    .Where(p => p.UserID == currentUser.Id)
                                                                    .Include(p => p.Contract)
                                                                    .ThenInclude(c => c.Institution)
                                                                    .Include(p => p.Contract)
                                                                    .ThenInclude(c => c.Client)
                                                                    .Include(p => p.Contract)
                                                                    .ThenInclude(c => c.Manager)
                                                                    .ToListAsync();
                List<Contract> parContracts = new();
                foreach(var item in participatings)
                {
                    parContracts.Add(item.Contract);
                }
                return View(parContracts);
            }
            return NotFound();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,InstitutionId,ConclusionDate,EfectiveDate,ExpiredDate,ManagerID,ClientID")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationNumber,InstitutionId,ConclusionDate,EfectiveDate,ExpiredDate,ManagerID,ClientID")] Contract contract)
        {
            if (id != contract.RegistrationNumber)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExist(contract.RegistrationNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool ContractExist(int id)
        {
            return _context.Contracts.Any(e => e.RegistrationNumber == id);
        }
    }
}
