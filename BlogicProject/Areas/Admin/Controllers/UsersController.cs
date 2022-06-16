using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using BlogicProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin))]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UsersController(UserManager<User> usermanager, AppDbContext context)
        {
            _userManager = usermanager;
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }
        

        public async Task<IActionResult> UserListRole(Roles? role)
        {
            if (role == null) return NotFound();

            var users = await _context.Users.Include(c => c.ParticipatesIn)
                                            .ThenInclude(pi => pi.Contract)
                                            .ToListAsync();
            IList<User> clients = new List<User>();
            foreach(var user in users)
            {
                if (_userManager.IsInRoleAsync(user, role.ToString()).Result)
                {
                    clients.Add(user);
                }
            }

            List<UserSimpleDetailViewModel> vms = new();
            foreach (var client in clients)
            {
                if (client == null)
                {
                    return NotFound();
                }

                var contractsClient = _context.Contracts.Where(c => c.Client == client).ToList();
                var contractsManager = _context.Contracts.Where(c => c.Manager == client).ToList();
                var participates = _context.Participatings.Where(p => p.UserID == client.Id).ToList();
                var roles = _userManager.GetRolesAsync(client).Result.ToList();


                UserSimpleDetailViewModel viewModel = new()
                {
                    User = client,
                    UserRoles = roles,
                    ContractsClient = contractsClient,
                    ContractsManager = contractsManager,
                    Participatings = client.ParticipatesIn
                };
                vms.Add(viewModel);
            }
            ViewBag.ForRole = role.ToString();
            return View(vms);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Edit/5
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

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
