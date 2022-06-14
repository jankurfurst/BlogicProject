using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace BlogicProject.Models.Database
{
    public class DatabaseInit
    {
        public void Initialization(AppDbContext appDbContext)
        {
            appDbContext.Database.EnsureCreated();

            //if (appDbContext.Contracts.Count() == 0)
            //{
            //    IList<Contract> cItems = GenerateContracts();
            //    foreach (var ci in cItems)
            //    {
            //        appDbContext.Contracts.Add(ci);
            //    }
            //    appDbContext.SaveChanges();
            //}
        }

        public List<Contract> GenerateContracts()
        {
            List<Contract> contracts = new List<Contract>();

            Contract c1 = new Contract()
            {
                ConclusionDate = DateTime.Now,
                EfectiveDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddYears(5),
                ManagerID = 2,
                ClientID = 3,
            };

            contracts.Add(c1);
            return contracts;
        }

        public List<Participating> GenerateParticipattings()
        {
            List<Participating> participatings = new List<Participating>();

            Participating p = new Participating()
            {
                UserID = 1,
                ContractID = 1,
            };

            participatings.Add(p);
            return participatings;
        }



        public async Task EnsureRoleCreated(RoleManager<Role> roleManager)
        {
            string[] roles = Enum.GetNames(typeof(Roles));

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }

        public async Task EnsureAdminCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "admin",
                Email = "admin@admin.cz",
                EmailConfirmed = true,
                PI_Number = "9912126633",
                FirstName = "Tom",
                LastName = "Správce"
            };
            string password = "abc";

            User adminInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (adminInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Admin: {error.Code}, {error.Description}");
                    }
                }
            }

        }

        public async Task EnsureManagerCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "manager",
                Email = "manager@manager.cz",
                EmailConfirmed = true,
                PI_Number = "9912126637",
                FirstName = "Tom",
                LastName = "Manažer"
            };
            string password = "abc";

            User managerInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (managerInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        if (role != Roles.Admin.ToString())
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Manager: {error.Code}, {error.Description}");
                    }
                }
            }

        }

        public async Task EnsureClientCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "client",
                Email = "klient@klient.cz",
                EmailConfirmed = true,
                PI_Number = "9912126638",
                FirstName = "Franta",
                LastName = "Klient"
            };
            string password = "abc";

            User managerInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (managerInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        if (role == Roles.Client.ToString())
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Manager: {error.Code}, {error.Description}");
                    }
                }
            }

        }
    }
}
