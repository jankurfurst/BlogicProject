using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;
using BlogicProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogicProject.Areas.Adviser.Controllers
{
    [Area("Adviser")]
    [Authorize(Roles = nameof(Roles.Adviser))]
    public class ContractsController : Controller
    {
        private readonly UserManager<User> _userManager;
        ISecurityApplicationService iSecure;
        private readonly AppDbContext _context;

        public ContractsController(UserManager<User> usermanager, ISecurityApplicationService iSecure, AppDbContext context)
        {
            _userManager = usermanager;
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
                foreach (var item in participatings)
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
        public async Task<IActionResult> Create()
        {
            ParticipantsViewModel vm = await FindParticipantsAsync();
            ContractViewModel cVM = new()
            {
                ConclusionDate = DateTime.Now,
                EfectiveDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddYears(5),
            };
            ViewBag.Partic = vm;
            var currentUser = iSecure.GetCurrentUser(User).Result;
            ViewBag.ManagerId = currentUser.Id;
            return View(cVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,InstitutionId,ConclusionDate,EfectiveDate,ExpiredDate,ManagerId,ClientId,ParticipatingAdvisers")] ContractViewModel contractVM)
        {
            if (ModelState.IsValid)
            {
                Contract contract = new()
                {
                    ConclusionDate = contractVM.ConclusionDate,
                    EfectiveDate = contractVM.EfectiveDate,
                    ExpiredDate = contractVM.ExpiredDate,
                    ManagerID = contractVM.ManagerId,
                    ClientID = contractVM.ClientId,
                    InstitutionId = contractVM.InstitutionId
                };
                _context.Add(contract);
                await _context.SaveChangesAsync();

                foreach (var adviserId in contractVM.ParticipatingAdvisers)
                {
                    if (adviserId != 0 && adviserId != contract.ManagerID)
                    {
                        Participating participating = new()
                        {
                            ContractID = contract.RegistrationNumber,
                            UserID = adviserId
                        };
                        _context.Add(participating);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ContractsController.ManagingContracts),
                                            nameof(ContractsController).Replace("Controller", String.Empty),
                                            new { area = "Adviser" });
            }

            ParticipantsViewModel vm = await FindParticipantsAsync();
            ViewBag.Partic = vm;
            var currentUser = iSecure.GetCurrentUser(User).Result;
            ViewBag.ManagerId = currentUser.Id;

            return View(contractVM);
        }
        #endregion

        #region Edit
        //Prepared
        public async Task<IActionResult> Edit(int? id)
        {
            //return NotFound();//delete this when done

            if (id == null)
            {
                return NotFound();
            }

            var contract = _context.Contracts.Include(c => c.Manager)
                                        .Include(c => c.Client)
                                        .Include(c => c.ParticipatesIn)
                                        .FirstOrDefaultAsync(c => c.RegistrationNumber == id).Result;
            if (contract == null)
            {
                return NotFound();
            }
            ContractViewModel cVM = new()
            {
                RegistrationNumber = contract.RegistrationNumber,
                ConclusionDate = contract.ConclusionDate,
                EfectiveDate = contract.EfectiveDate,
                ExpiredDate = contract.ExpiredDate,
                ManagerId = contract.ManagerID,
                ClientId = contract.ClientID,
                InstitutionId = contract.InstitutionId
            };

            ParticipantsViewModel vm = await FindParticipantsAsync();
            var currentUser = iSecure.GetCurrentUser(User).Result;
            ViewBag.ManagerId = currentUser.Id;
            ViewBag.Partic = vm;

            return View(cVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationNumber,InstitutionId,ConclusionDate,EfectiveDate,ExpiredDate,ManagerId,ClientId,ParticipatingAdvisers")] ContractViewModel contractVM)
        {
            if (id != contractVM.RegistrationNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!ContractExist(contractVM.RegistrationNumber))
                {
                    return NotFound();
                }
                var contract = new Contract
                {
                    RegistrationNumber = contractVM.RegistrationNumber,
                    ConclusionDate = contractVM.ConclusionDate,
                    EfectiveDate = contractVM.EfectiveDate,
                    ExpiredDate = contractVM.ExpiredDate,
                    InstitutionId = contractVM.InstitutionId,
                    ManagerID = contractVM.ManagerId,
                    ClientID = contractVM.ClientId
                };

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
                return RedirectToAction(nameof(ContractsController.ManagingContracts),
                                            nameof(ContractsController).Replace("Controller", String.Empty),
                                            new { area = "Adviser" });
            }

            ParticipantsViewModel vm = await FindParticipantsAsync();
            ViewBag.Partic = vm;
            var currentUser = iSecure.GetCurrentUser(User).Result;
            ViewBag.ManagerId = currentUser.Id;
            return View(contractVM);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.Include(c => c.Manager)
                                            .Include(c => c.Client)
                                            .Include(c => c.Institution)
                                            .Include(c => c.ParticipatesIn)
                                            .ThenInclude(pi => pi.User)
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
            //_context.Contracts.Remove(contract);
            _context.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ContractsController.ManagingContracts),
                                            nameof(ContractsController).Replace("Controller", String.Empty),
                                            new { area = "Adviser" });
        }
        #endregion

        private async Task<ParticipantsViewModel> FindParticipantsAsync()
        {
            ParticipantsViewModel vm = new()
            {
                Advisers = new List<User>(),
                Clients = new List<User>(),
                Managers = new List<User>(),
                Institutions = _context.Institutions.ToList()
            };
            var currentUser = iSecure.GetCurrentUser(User).Result;
            var users = await _context.Users.Include(c => c.ParticipatesIn)
                                            .ThenInclude(pi => pi.Contract)
                                            .ToListAsync();
            foreach (var user in users)
            {
                if (_userManager.IsInRoleAsync(user, Roles.Client.ToString()).Result && user.Id != currentUser.Id)
                {
                    vm.Clients.Add(user);
                }

                if (_userManager.IsInRoleAsync(user, Roles.Adviser.ToString()).Result)
                {
                    if (user.Id != currentUser.Id)
                    {
                        vm.Advisers.Add(user);
                    }
                    vm.Managers.Add(user);
                }
            }
            return vm;
        }

        private bool ContractExist(int id)
        {
            return _context.Contracts.Any(e => e.RegistrationNumber == id);
        }
    }
}
