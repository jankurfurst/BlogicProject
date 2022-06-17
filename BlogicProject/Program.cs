using BlogicProject.Models.ApplicationServices.Abstraction;
using BlogicProject.Models.ApplicationServices.Implementation;
using BlogicProject.Models.Database;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews();
var Configuration = builder.Configuration;

builder.Services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("database")));
//builder.Services.AddDbContextPool<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString("mysql"), new MySqlServerVersion("8.0.26")));

builder.Services.AddIdentity<User, Role>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 2;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Security/Account/Login";
    options.LogoutPath = "/Security/Account/Logout";
    options.SlidingExpiration = true;
});


builder.Services.AddScoped<ISecurityApplicationService, SecurityIdentityApplicationService>();

builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>

{
    // Set a short timeout for easy testing.
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    if (scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        DatabaseInit dbInit = new DatabaseInit();
        //dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        using (Task task = dbInit.EnsureRoleCreated(roleManager))
        {
            task.Wait();
        }

        using (Task task = dbInit.EnsureAdminCreated(userManager))
        {
            task.Wait();
        }

        using (Task task = dbInit.EnsureManagerCreated(userManager))
        {
            task.Wait();
        }

        using (Task task = dbInit.EnsureClientCreated(userManager))
        {
            task.Wait();
        }
        dbInit.Initialization(dbContext);
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute
    (
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute
    (
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
