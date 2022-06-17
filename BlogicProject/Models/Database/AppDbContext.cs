using BlogicProject.Models.Entity;
using BlogicProject.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogicProject.Models.Database
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Participating> Participatings { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Participating>()
                .HasOne(c => c.Contract)
                .WithMany(pi => pi.ParticipatesIn)
                .HasForeignKey(ci => ci.ContractID);

            modelBuilder.Entity<Participating>()
                .HasOne(u => u.User)
                .WithMany(pi => pi.ParticipatesIn)
                .HasForeignKey(ui => ui.UserID);
            
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().Replace("AspNet", String.Empty));
            }
        }
    }
}
