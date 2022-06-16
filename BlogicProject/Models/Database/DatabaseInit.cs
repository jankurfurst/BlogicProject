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

            if (!appDbContext.Contracts.Any() && !appDbContext.Institutions.Any() && !appDbContext.Participatings.Any())
            {
                IList<Institution> iItems = GenerateInstitutions();
                foreach (var i in iItems)
                {
                    appDbContext.Institutions.Add(i);
                }
                appDbContext.SaveChanges();

                IList<Contract> cItems = GenerateContracts();
                foreach (var c in cItems)
                {
                    appDbContext.Contracts.Add(c);
                }
                appDbContext.SaveChanges();

                IList<Participating> pItems = GenerateParticipattings();
                foreach (var p in pItems)
                {
                    appDbContext.Participatings.Add(p);
                }

                appDbContext.SaveChanges();
            }
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
                InstitutionId = 1
            };
            contracts.Add(c1);
            
            Contract c2 = new Contract()
            {
                ConclusionDate = DateTime.Now,
                EfectiveDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddYears(5),
                ManagerID = 1,
                ClientID = 3,
                InstitutionId = 1
            };
            contracts.Add(c2);
            
            Contract c3 = new Contract()
            {
                ConclusionDate = DateTime.Now,
                EfectiveDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddYears(5),
                ManagerID = 1,
                ClientID = 2,
                InstitutionId = 1
            };
            contracts.Add(c3);


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

        public List<Institution> GenerateInstitutions()
        {
            List<Institution> institutions = new List<Institution>();

            Institution i = new()
            {
                Name = "Axa",
            };

            institutions.Add(i);
            return institutions;
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
                LastName = "Správce",
                PhoneNumber = "123456789"
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
                FirstName = "Alex",
                LastName = "Manažer",
                PhoneNumber = "987654321"
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
                        Debug.WriteLine($"Error during Role creation for Adviser: {error.Code}, {error.Description}");
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
                LastName = "Klient",
                PhoneNumber = "456123789"
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
                        Debug.WriteLine($"Error during Role creation for Client: {error.Code}, {error.Description}");
                    }
                }
            }

        }
    }
}
