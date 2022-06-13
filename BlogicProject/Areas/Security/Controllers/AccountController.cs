using BlogicProject.Controllers;
using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using BlogicProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogicProjectAreas.Security.Controllers
{
    [Area("Security")]
    public class AccountController : Controller
    {
        ISecurityApplicationService security;
        private readonly AppDbContext _context;
        private readonly UserManager<User> userManager;

        public AccountController(UserManager<User> userManager, AppDbContext context, ISecurityApplicationService security)
        {
            this.security = security;
            _context = context;
            this.userManager = userManager;
        }


        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                string[] errors = await security.Register(registerVM, BlogicProject.Models.Identity.Roles.Client);

                if (errors == null)
                {
                    LoginViewModel loginVM = new LoginViewModel
                    {
                        Username = registerVM.Username,
                        Password = registerVM.Password
                    };
                    bool isLogged = await security.Login(loginVM);
                    if (isLogged)
                        return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", String.Empty), new { area = String.Empty });
                    else
                        return RedirectToAction(nameof(Login));
                
                }
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

            }
            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                bool isLogged = await security.Login(loginVM);
                if (isLogged)
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", String.Empty), new { area = String.Empty });

                loginVM.LoginFailed = true;
            }
            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await security.Logout();
            return RedirectToAction(nameof(Login));
        }
    }
}
